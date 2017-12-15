using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Lucene.Net.Search;
using Newtonsoft.Json;
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

            var jsonObject = JObject.Parse(content);

            var biotope = JsonConvert.DeserializeObject<WEB_BIOTOPE>(jsonObject["Biotope"].First.ToString());
//            var species = JsonConvert.DeserializeObject<List<WEB_BIOT_SPECIES_OBSERVATION>>(jsonObject["Species"].ToString());
            var similarBiotopes = JsonConvert.DeserializeObject<List<WEB_BIOT_RELATION>>(jsonObject["SimilarBiotopes"].ToString());
            var oldCodes = JsonConvert.DeserializeObject<List<WEB_OLD_CODE>>(jsonObject["OldCodes"].ToString());

            var biotopeModel = new BiotopeModel(model.Content)
            {
                Biotope = biotope,
//                Species = species,
                SimilarBiotopes = similarBiotopes,
                OldCodes = oldCodes
            };

            return CurrentTemplate(biotopeModel);
        }
    }
}