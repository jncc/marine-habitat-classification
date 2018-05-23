using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using MHC.Umbraco.Plugin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace MHC.Umbraco.Plugin.Controllers
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
            var url = ConfigurationManager.AppSettings["MicroserviceUrl"] + "/Biotope/" + key;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            var biotopeModel = new BiotopeModel(model.Content)
            {
                Biotope = null,
                Species = null,
                BiotopeHierarchy = null,
                SimilarBiotopes = null,
                OldCodes = null,
                HabitatCorrelations = null,
                Photos = null
            };

            try
            {
                var response = (HttpWebResponse) request.GetResponse();

                string content;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    content = sr.ReadToEnd();
                }

                biotopeModel = GetBiotopeModel(content, model.Content);
            }
            catch (WebException we)
            {
//                return View("Error", Umbraco.GetErrorPage());
                //TODO: error page handling?
            }

            return CurrentTemplate(biotopeModel);
        }

        private BiotopeModel GetBiotopeModel(string jsonContent, IPublishedContent modelContent)
        {
            var jsonObject = JObject.Parse(jsonContent);

            var biotope = JsonConvert.DeserializeObject<Biotope>(jsonObject["Biotope"].ToString());
            var species = JsonConvert.DeserializeObject<List<Species>>(jsonObject["Species"].ToString());
            var biotopeHierarchy =
                JsonConvert.DeserializeObject<Dictionary<int, BiotopeLevel>>(jsonObject["BiotopeHierarchy"]
                    .ToString());
            var similarBiotopes =
                JsonConvert.DeserializeObject<List<SimilarBiotope>>(jsonObject["SimilarBiotopes"].ToString());
            var oldCodes = JsonConvert.DeserializeObject<List<OldCode>>(jsonObject["OldCodes"].ToString());
            var habitatCorrelations = JsonConvert.DeserializeObject<List<HabitatCorrelation>>(jsonObject["HabitatCorrelations"].ToString());
            var photos = JsonConvert.DeserializeObject<List<Photo>>(jsonObject["Photos"].ToString());

            PopulateFullTypicalAbundanceTerms(species);
            species.Sort((species1, species2) => string.Compare(species1.Sort, species2.Sort, StringComparison.Ordinal));

            var biotopeModel = new BiotopeModel(modelContent)
            {
                Biotope = biotope,
                Species = species,
                BiotopeHierarchy = biotopeHierarchy,
                SimilarBiotopes = similarBiotopes,
                OldCodes = oldCodes,
                HabitatCorrelations = habitatCorrelations,
                Photos = photos
            };

            return biotopeModel;
        }

        private void PopulateFullTypicalAbundanceTerms(List<Species> speciesList)
        {
            foreach (var species in speciesList)
            {
                AbundanceCodes.TryGetValue(species.TypicalAbundance, out var fullTerm);
                species.TypicalAbundance = fullTerm;
            }
        }

        public static readonly Dictionary<string, string> AbundanceCodes = new Dictionary<string, string>
        {
            {"S", "Super abundant"},
            {"A", "Abundant"},
            {"C", "Common"},
            {"F", "Frequent"},
            {"O", "Occasional"},
            {"R", "Rare"},
        };
    }
}