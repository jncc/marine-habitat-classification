using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace website.Controllers
{
    public class BiotopeController : RenderMvcController
    {
        // GET: Biotopes
        public override ActionResult Index(RenderModel model)
        {
            //            return base.Index(model);
            return Json("some json data", JsonRequestBehavior.AllowGet);
        }

        // GET: Biotopes/biotopeKey
        public ActionResult Biotope(RenderModel model, string key)
        {
            var url = "http://localhost:54760/Biotope/" + key;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string content;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                content = sr.ReadToEnd();
            }

            return Json(content, JsonRequestBehavior.AllowGet);
            //var biotopeModel = new BiotopeModel(model.Content) { Key = key };
            //return CurrentTemplate(biotopeModel);
        }
    }
}