using microservices.Clients;
using microservices.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace microservices.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public string Index()
        {
            return "Search home";
        }
        
        [HttpPost]
        [Route("search/index/mhc")]
        public string RefreshMhcIndex()
        {
            var baseUrl = ConfigurationManager.AppSettings["FlexSearchUrl"];
            
            DeleteAllDocuments(baseUrl);
            DeleteCurrentIndex(baseUrl);
            CreateBiotopeIndex(baseUrl);

            using (var db = new BiotopeDB())
            {
                db.Configuration.LazyLoadingEnabled = true;
                db.Configuration.ProxyCreationEnabled = true;

                foreach (var biotope in db.WEB_BIOTOPE)
                {
                    CreateBiotopeDocuments(biotope, baseUrl);
                }

                return $"Indexing completed successfully, {db.WEB_BIOTOPE.Count()} biotopes indexed";
            }
        }

        [HttpPost]
        [Route("search/index/jncc")]
        public async Task<string> RefreshJnccIndex()
        {
            await AddBiotopesToQueue();
            return "Indexing completed successfully";
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

        private async Task AddBiotopesToQueue()
        {
            var env = new Env();

            using (var db = new BiotopeDB())
            {
                db.Configuration.LazyLoadingEnabled = true;
                db.Configuration.ProxyCreationEnabled = true;

                using (var client = new QueueClient(env))
                {
                    var htmlTagRegex = "<.*?>";
                    foreach (var biotope in db.WEB_BIOTOPE)
                    {
                        var formattedTitle = Regex.Replace(biotope.FULL_TERM, htmlTagRegex, String.Empty);
                        var formattedDescription = string.IsNullOrWhiteSpace(biotope.DESCRIPTION) ? null : Regex.Replace(biotope.DESCRIPTION, htmlTagRegex, String.Empty);
                        var formattedSituation = string.IsNullOrWhiteSpace(biotope.SITUATION) ? null : Regex.Replace(biotope.SITUATION, htmlTagRegex, String.Empty);

                        var message = new
                        {
                            verb = "upsert",
                            index = env.QUEUE_INDEX,
                            document = new
                            {
                                id = biotope.BIOTOPE_KEY,
                                site = env.SITE,
                                title = $"{biotope.ORIGINAL_CODE} {formattedTitle}",
                                content = $"{formattedDescription} {formattedSituation} {GetSpeciesString(biotope, htmlTagRegex)}",
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

                        await client.Send(jsonString);
                    }
                }
            }
        }

        private string GetSpeciesString(WEB_BIOTOPE biotope, string regex)
        {
            var species = new List<string>();

            foreach (var grabSpecies in biotope.WEB_BIOT_SPECIES_GRAB)
            {
                var formattedSpecies = Regex.Replace(grabSpecies.ITEM_NAME, regex, String.Empty);

                if (!species.Contains(formattedSpecies))
                {
                    species.Add(formattedSpecies);
                }
            }

            foreach (var observationSpecies in biotope.WEB_BIOT_SPECIES_OBSERVATION)
            {
                var formattedSpecies = Regex.Replace(observationSpecies.ITEM_NAME, regex, String.Empty);

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
                    return level.BIOTOPE_PARENT_KEY;
                }
            }

            return null;
        }
    }
}