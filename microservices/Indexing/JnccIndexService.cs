using Aws4RequestSigner;
using microservices.Clients;
using microservices.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
}