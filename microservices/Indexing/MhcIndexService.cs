using System.Net;
using System.Text;
using microservices.Models;

namespace microservices.Indexing
{
    public interface IMhcIndexService
    {
        void ResetIndex();
        void AddBiotope(string biotope);
    }

    public class MhcIndexService : IMhcIndexService
    {
        private readonly Env env;

        public MhcIndexService()
        {
            this.env = new Env();
        }

        public void ResetIndex()
        {
            DeleteAllDocuments();
            DeleteCurrentIndex();
            CreateBiotopeIndex();
        }

        public void AddBiotope(string biotope)
        {
            var request = (HttpWebRequest)WebRequest.Create(env.FLEXSEARCH_URL + "/indices/biotope/documents");
            request.Method = "POST";

            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(biotope);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.GetResponse();
        }

        private void DeleteAllDocuments()
        {
            var request = (HttpWebRequest)WebRequest.Create(env.FLEXSEARCH_URL + "/indices/biotope/documents");
            request.Method = "DELETE";

            request.GetResponse();
        }

        private void DeleteCurrentIndex()
        {
            var request = (HttpWebRequest)WebRequest.Create(env.FLEXSEARCH_URL + "/indices/biotope");
            request.Method = "DELETE";

            request.GetResponse();
        }

        private void CreateBiotopeIndex()
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

            var request = (HttpWebRequest)WebRequest.Create(env.FLEXSEARCH_URL + "/indices");
            request.Method = "POST";

            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(biotopeIndex);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.GetResponse();
        }
    }
}