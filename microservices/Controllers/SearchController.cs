using microservices.Indexing;
using microservices.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using BiotopeDB = microservices.Models.BiotopeDB;

namespace microservices.Controllers
{
    public class SearchController : Controller
    {
        [HttpPost]
        [Route("search/index_mhc")]
        public string RefreshMhcIndex()
        {
            var mhcIndexService = new MhcIndexService();

            mhcIndexService.ResetIndex();

            using (var db = new BiotopeDB())
            {
                db.Configuration.LazyLoadingEnabled = true;
                db.Configuration.ProxyCreationEnabled = true;

                foreach (var biotope in db.WEB_BIOTOPE)
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

                    mhcIndexService.AddBiotope(biotopeDoc);
                }

                return $"Indexing completed successfully, {db.WEB_BIOTOPE.Count()} biotopes indexed";
            }
        }

        private static string htmlTagRegex = "<.*?>";

        [HttpPost]
        [Route("search/clear_jncc_index")]
        public async Task<string> ClearJnccIndex()
        {
            var jnccIndexService = new JnccIndexService();
            await jnccIndexService.ClearIndex();

            return "JNCC index cleared successfully";
        }

        [HttpPost]
        [Route("search/populate_jncc_index")]
        public async Task<string> PopulateJnccIndex()
        {
            var env = new Env();
            var jnccIndexService = new JnccIndexService();
            var biotopesList = new List<string>();

            using (var db = new BiotopeDB())
            {
                foreach (var biotope in db.WEB_BIOTOPE)
                {
                    var formattedTitle = Regex.Replace(biotope.FULL_TERM, htmlTagRegex, String.Empty);
                    var formattedDescription = string.IsNullOrWhiteSpace(biotope.DESCRIPTION) ? null : Regex.Replace(biotope.DESCRIPTION, htmlTagRegex, String.Empty);
                    var formattedSituation = string.IsNullOrWhiteSpace(biotope.SITUATION) ? null : Regex.Replace(biotope.SITUATION, htmlTagRegex, String.Empty);

                    var message = new
                    {
                        verb = "upsert",
                        index = env.SEARCH_INDEX,
                        document = new
                        {
                            id = biotope.BIOTOPE_KEY,
                            site = env.SITE,
                            title = $"{biotope.ORIGINAL_CODE} {formattedTitle}",
                            content = $"{formattedDescription} {formattedSituation} {GetSpeciesString(biotope)}",
                            url = env.BIOTOPE_BASE_URL + biotope.BIOTOPE_KEY.ToLower(),
                            resource_type = "dataset",
                            keywords = GetKeywords(biotope),
                            published_date = "2015-03"
                        }
                    };

                    var jsonString = JsonConvert.SerializeObject(message, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                    biotopesList.Add(jsonString);
                }
            }

            await jnccIndexService.AddBiotopes(biotopesList);

            return "JNCC indexing completed successfully";
        }

        private string GetSpeciesString(WEB_BIOTOPE biotope)
        {
            var species = new List<string>();

            foreach (var grabSpecies in biotope.WEB_BIOT_SPECIES_GRAB)
            {
                var formattedSpecies = Regex.Replace(grabSpecies.ITEM_NAME, htmlTagRegex, String.Empty);

                if (!species.Contains(formattedSpecies))
                {
                    species.Add(formattedSpecies);
                }
            }

            foreach (var observationSpecies in biotope.WEB_BIOT_SPECIES_OBSERVATION)
            {
                var formattedSpecies = Regex.Replace(observationSpecies.ITEM_NAME, htmlTagRegex, String.Empty);

                if (!species.Contains(formattedSpecies))
                {
                    species.Add(formattedSpecies);
                }
            }

            return string.Join(", ", species);
        }

        private object[] GetKeywords(WEB_BIOTOPE biotope)
        {
            var rootBiotope = GetRootBiotope(biotope.WEB_BIOTOPE_HIERARCHY);
            if (!string.IsNullOrWhiteSpace(rootBiotope))
            {
                return new[]
                {
                    new
                    {
                        vocab = "http://vocab.jncc.gov.uk/mhc",
                        value = "biotope"
                    },
                    new
                    {
                        vocab = "http://vocab.jncc.gov.uk/mhc",
                        value = rootBiotope
                    }
                };
            }
            else
            {
                return new[]
                {
                    new
                    {
                        vocab = "http://vocab.jncc.gov.uk/mhc",
                        value = "biotope"
                    }
                };
            }
        }

        private string GetRootBiotope(ICollection<WEB_BIOTOPE_HIERARCHY> biotopeHierarchy)
        {
            foreach (var level in biotopeHierarchy)
            {
                if (level.HIGHERLEVEL == 1)
                {
                    return Regex.Replace(level.WEB_BIOTOPE1.FULL_TERM, htmlTagRegex, String.Empty);
                }
            }

            return null;
        }
    }
}