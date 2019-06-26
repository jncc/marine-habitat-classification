using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using website.Models;
using System.Linq;

namespace website.Controllers
{
    public class BiotopesController : RenderMvcController
    {

        // GET: Biotopes/biotopeKey
        public ActionResult Biotopes(RenderModel model, string key)
        {
            var env = new Env();
            var url = env.MICROSERVICE_URL + "/biotope/" + key;
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
            catch (Exception)
            {
                return View("Error", Umbraco.GetErrorPage());
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
            species = species.OrderBy(s => s.Sort).ToList();

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
                var fullTerm = "";
                if (!string.IsNullOrWhiteSpace(species.TypicalAbundance))
                {
                    AbundanceCodes.TryGetValue(species.TypicalAbundance, out fullTerm);
                }
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