using System.Configuration;
using System.Net;
using System.Text;
using System.Web.Mvc;
using microservices.Models;
using WebGrease.Css.Extensions;

namespace microservices.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public string Index()
        {
            return "Search home";
        }

        // POST api/values
        [HttpPost]
        [Route("search/index")]
        public string IndexBiotopes()
        {
            var baseUrl = ConfigurationManager.AppSettings["FlexSearchUrl"];

            try
            {
                DeleteAllDocuments(baseUrl);
                DeleteCurrentIndex(baseUrl);
                CreateBiotopeIndex(baseUrl);
            }
            catch (WebException we)
            {
                return "An error occurred during indexing: " + we.Message;
            }

            var failures = 0;
            try
            {
                using (var db = new BiotopeDB())
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    db.Configuration.ProxyCreationEnabled = true;

                    db.WEB_BIOTOPE.ForEach(b => CreateBiotopeDocuments(b, baseUrl));
                }
            }
            catch (WebException)
            {
                failures++;
            }

            return "Indexing complete\r\n" +
                   "Number of biotopes that failed processing: " + failures;
        }

        private void DeleteAllDocuments(string baseUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "/indices/biotope/documents");
            request.Method = "DELETE";

            request.GetResponse();
        }

        private void DeleteCurrentIndex(string baseUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "/indices/biotope");
            request.Method = "DELETE";

            request.GetResponse();
        }

        private void CreateBiotopeIndex(string baseUrl)
        {
            var biotopeIndex = @"{
                ""indexName"": ""biotope"",
                ""fields"": [
                {
                    ""fieldName"": ""originalCode""
                },
                {
                    ""fieldName"": ""fullTerm""
                },
                {
                    ""fieldName"": ""description""
                },
                {
                    ""fieldName"": ""hierarchyLevel"",
                    ""fieldType"": ""Stored""
                }
                ]
            }";

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "/indices");
            request.Method = "POST";

            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(biotopeIndex);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.GetResponse();
        }

        private void CreateBiotopeDocuments(WEB_BIOTOPE biotope, string baseUrl)
        {
            // Replace " with \" or the json will break
            var jsonSafeDescription = biotope.DESCRIPTION.Replace("\"", "\\\"");

            // Replacing full stops in original code with spaces because I can't figure out how to use a lettertokenizer
            var biotopeDoc = $@"{{
                ""fields"": {{
                    ""originalCode"": ""{biotope.ORIGINAL_CODE.Trim().Replace('.', ' ')}"",
                    ""fullTerm"": ""{biotope.FULL_TERM}"",
                    ""description"": ""{jsonSafeDescription}"",
                    ""hierarchyLevel"": ""{biotope.WEB_BIOTOPE_HIERARCHY.Count}""
                }},
                ""id"": ""{biotope.BIOTOPE_KEY}"",
                indexName: ""biotope""
            }}";


            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "/indices/biotope/documents");
            request.Method = "POST";

            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(biotopeDoc);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.GetResponse();
        }
    }
}