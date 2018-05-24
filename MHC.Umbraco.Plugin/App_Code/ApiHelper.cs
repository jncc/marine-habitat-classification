using System.Configuration;
using System.IO;
using System.Net;

namespace website.App_Code
{
    public static class ApiHelper
    {
        public static string GetBiotopeFromApi(string biotopeKey)
        {
            var url = ConfigurationManager.AppSettings["BiotopeApiUrl"] + "/Biotope/" + biotopeKey;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();

            string content;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                content = sr.ReadToEnd();
            }

            return content;
        }

        public static string GetBiotopeHierarchyFromApi()
        {
            var url = ConfigurationManager.AppSettings["BiotopeApiUrl"] + "/Biotope/";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();

            string content;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                content = sr.ReadToEnd();
            }

            return content;
        }
    }
}