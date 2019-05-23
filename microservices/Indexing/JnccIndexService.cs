﻿using microservices.Clients;
using microservices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace microservices.Indexing
{
    public interface IJnccIndexService
    {
        Task ClearIndex();
        Task AddBiotopes(List<string> biotopes);
    }

    public class JnccIndexService : IJnccIndexService
    {
        private static readonly HttpClient httpClient;

        static JnccIndexService()
        {
            httpClient = new HttpClient();
        }

        public async Task ClearIndex()
        {
            var response = await PostDeleteRequest();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error clearing mhc index: {response.StatusCode} {response.RequestMessage}");
            }
        }

        public async Task AddBiotopes(List<string> biotopes)
        {
            using (var client = new QueueClient(new Env()))
            {
                foreach (var biotope in biotopes)
                {
                    await client.Send(biotope);
                }
            }
        }

        private async Task<HttpResponseMessage> PostDeleteRequest()
        {
            var env = new Env();
            var body = "{\"query\": {\"match\": {\"site\": \"mhc\"}}}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(env.ES_ENDPOINT + "_delete_by_query"),
                Content = new StringContent(
                    body,
                    Encoding.UTF8,
                    "application/json"
                )
            };

            var signedRequest = await GetSignedRequest(request, env);
            var response = await httpClient.SendAsync(signedRequest);
            var responseMessage = response;

            return responseMessage;
        }

        private async Task<HttpRequestMessage> GetSignedRequest(HttpRequestMessage request, Env env)
        {
            var signer = new AWS4RequestSigner(env.SEARCH_AWS_ACCESSKEY, env.SEARCH_AWS_SECRETACCESSKEY);
            return await signer.Sign(request, "es", env.SEARCH_AWS_REGION);
        }
    }

    /**
     * AWS4RequestSigner reluctantly copied from https://github.com/tsibelman/aws-signer-v4-dot-net (v0.4.0.0), not our code!!
     * For some reason that we've yet to figure out, the same code from the nuget package throws a MissingMethodException at runtime
     * on the test server, while copying the code like below works fine.
     */
    public class AWS4RequestSigner
    {
        private readonly string _access_key;
        private readonly string _secret_key;
        private readonly SHA256 _sha256;
        private const string algorithm = "AWS4-HMAC-SHA256";

        public AWS4RequestSigner(string accessKey, string secretKey)
        {

            if (string.IsNullOrEmpty(accessKey))
            {
                throw new ArgumentOutOfRangeException(nameof(accessKey), accessKey, "Not a valid access_key.");
            }

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentOutOfRangeException(nameof(secretKey), secretKey, "Not a valid secret_key.");
            }

            _access_key = accessKey;
            _secret_key = secretKey;
            _sha256 = SHA256.Create();
        }

        private string Hash(byte[] bytesToHash)
        {
            var result = _sha256.ComputeHash(bytesToHash);
            return ToHexString(result);
        }

        private static byte[] HmacSHA256(byte[] key, string data)
        {
            var hashAlgorithm = new HMACSHA256(key);

            return hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes("AWS4" + key);
            byte[] kDate = HmacSHA256(kSecret, dateStamp);
            byte[] kRegion = HmacSHA256(kDate, regionName);
            byte[] kService = HmacSHA256(kRegion, serviceName);
            byte[] kSigning = HmacSHA256(kService, "aws4_request");
            return kSigning;
        }

        private static string ToHexString(byte[] array)
        {
            var hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public async Task<HttpRequestMessage> Sign(HttpRequestMessage request, string service, string region)
        {
            if (string.IsNullOrEmpty(service))
            {
                throw new ArgumentOutOfRangeException(nameof(service), service, "Not a valid service.");
            }

            if (string.IsNullOrEmpty(region))
            {
                throw new ArgumentOutOfRangeException(nameof(region), region, "Not a valid region.");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Headers.Host == null)
            {
                request.Headers.Host = request.RequestUri.Host;
            }

            var t = DateTimeOffset.UtcNow;
            var amzdate = t.ToString("yyyyMMddTHHmmssZ");
            request.Headers.Add("x-amz-date", amzdate);
            var datestamp = t.ToString("yyyyMMdd");

            var canonical_request = new StringBuilder();
            canonical_request.Append(request.Method + "\n");
            canonical_request.Append(request.RequestUri.AbsolutePath + "\n");

            var canonicalQueryParams = GetCanonicalQueryParams(request);

            canonical_request.Append(canonicalQueryParams + "\n");

            var signedHeadersList = new List<string>();

            foreach (var header in request.Headers.OrderBy(a => a.Key.ToLowerInvariant()))
            {
                canonical_request.Append(header.Key.ToLowerInvariant());
                canonical_request.Append(":");
                canonical_request.Append(string.Join(",", header.Value.Select(s => s.Trim())));
                canonical_request.Append("\n");
                signedHeadersList.Add(header.Key.ToLowerInvariant());
            }

            canonical_request.Append("\n");

            var signed_headers = string.Join(";", signedHeadersList);

            canonical_request.Append(signed_headers + "\n");

            var content = new byte[0];
            if (request.Content != null)
            {
                content = await request.Content.ReadAsByteArrayAsync();
            }
            var payload_hash = Hash(content);

            canonical_request.Append(payload_hash);

            var credential_scope = $"{datestamp}/{region}/{service}/aws4_request";

            var string_to_sign = $"{algorithm}\n{amzdate}\n{credential_scope}\n" + Hash(Encoding.UTF8.GetBytes(canonical_request.ToString()));

            var signing_key = GetSignatureKey(_secret_key, datestamp, region, service);
            var signature = ToHexString(HmacSHA256(signing_key, string_to_sign));

            request.Headers.TryAddWithoutValidation("Authorization", $"{algorithm} Credential={_access_key}/{credential_scope}, SignedHeaders={signed_headers}, Signature={signature}");

            return request;
        }

        private static string GetCanonicalQueryParams(HttpRequestMessage request)
        {
            var querystring = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var keys = querystring.AllKeys.OrderBy(a => a).ToArray();

            // Query params must be escaped in upper case (i.e. "%2C", not "%2c").
            var queryParams = keys.Select(key => $"{key}={Uri.EscapeDataString(querystring[key])}");
            var canonicalQueryParams = string.Join("&", queryParams);
            return canonicalQueryParams;
        }
    }
}