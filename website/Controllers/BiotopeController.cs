using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using website.Models;

namespace website.Controllers
{
    public class BiotopeController : RenderMvcController
    {
        // GET: Biotope
        public override ActionResult Index(RenderModel model)
        {
            return Json("some json data", JsonRequestBehavior.AllowGet);
        }

        // GET: Biotope/biotopeKey
        public ActionResult Biotope(RenderModel model, string key)
        {
            var url = "http://localhost:54760/Biotope/" + key;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();
            string content;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                content = sr.ReadToEnd();
            }

            var json = JArray.Parse(content).First;

            var biotopeModel = new BiotopeModel(model.Content)
            {
                Key = json.Value<string>("BIOTOPE_KEY"),
                Description = json.Value<string>("DESCRIPTION")
            };

            return CurrentTemplate(biotopeModel);
        }
    }
}