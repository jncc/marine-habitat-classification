using System.Configuration;
using System.Linq;
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
            var totalCount = 0;
            try
            {
                    using (var db = new BiotopeDB())
                {
                    totalCount = db.WEB_BIOTOPE.Count();
                    db.WEB_BIOTOPE.ForEach(b => CreateBiotopeDocuments(b, baseUrl));
                }
            }
            catch (WebException we)
            {
                failures++;
            }

            return "Indexing complete\r\n" +
                   "There were " + failures + " failures out of a total " + totalCount + " biotopes indexed";
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
                    ""fieldName"": ""biotopeKey"",
                    ""fieldType"": ""Keyword""
                },
                {
                    ""fieldName"": ""originalCode""
                },
                {
                    ""fieldName"": ""fullTerm""
                },
                {
                    ""fieldName"": ""description""
                }
                ]
            }";

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "/indices");
            request.Method = "POST";

            ASCIIEncoding encoding = new ASCIIEncoding();
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

            var biotopeDoc = $@"{{
                ""fields"": {{
                    ""biotopeKey"": '{biotope.BIOTOPE_KEY}',
                    ""originalCode"": ""{biotope.ORIGINAL_CODE}"",
                    ""fullTerm"": ""{biotope.FULL_TERM}"",
                    ""description"": ""{jsonSafeDescription}""
                }},
                ""id"": ""{biotope.BIOTOPE_KEY}"",
                indexName: ""biotope""
            }}";


            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "/indices/biotope/documents");
            request.Method = "POST";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(biotopeDoc);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.GetResponse();
        }
    }
}