using microservices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace microservices.Controllers
{
    public class BiotopeController : Controller
    {
        // GET: Biotope
        public JsonResult Index()
        {
            using (var db = new BiotopeDB())
            {
                var data = db.WEB_BIOTOPE_HIERARCHY.Select(b => new { b.BIOTOPE_KEY, b.BIOTOPE_PARENT_KEY, b.HIGHERLEVEL, b.LEVEL, b.WEB_BIOTOPE, b.WEB_BIOTOPE1 }).ToList();
                var parentChildHierarchy = new List<object>();

                foreach (var hierarchyLevel in data)
                {
                    if (hierarchyLevel.LEVEL == hierarchyLevel.HIGHERLEVEL + 1)
                    {
                        parentChildHierarchy.Add(new 
                        {
                            BiotopeKey = hierarchyLevel.BIOTOPE_KEY,
                            BiotopeParentKey = hierarchyLevel.BIOTOPE_PARENT_KEY,
                            FullTerm = hierarchyLevel.WEB_BIOTOPE.FULL_TERM,
                            SortCode = hierarchyLevel.WEB_BIOTOPE.SORT_CODE
                        });
                    } else if (hierarchyLevel.LEVEL == 1 && hierarchyLevel.HIGHERLEVEL == 1)
                    {
                        parentChildHierarchy.Add(new
                        {
                            BiotopeKey = hierarchyLevel.BIOTOPE_KEY,
                            BiotopeParentKey = hierarchyLevel.BIOTOPE_KEY,
                            FullTerm = hierarchyLevel.WEB_BIOTOPE.FULL_TERM,
                            SortCode = hierarchyLevel.WEB_BIOTOPE.SORT_CODE
                        });
                    }
                }

                return Json(parentChildHierarchy, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Biotope/biotopeKey
        [Route("Biotope/{key}")]
        public JsonResult Index(string key)
        {
            using (var db = new BiotopeDB())
            {
                db.Configuration.LazyLoadingEnabled = true;
                db.Configuration.ProxyCreationEnabled = true;

                var biotope = db.WEB_BIOTOPE.Where(b => b.BIOTOPE_KEY == key).ToList();
                var similarBiotopes = db.WEB_BIOT_RELATION.Where(b => b.BIOTOPE_KEY == key).ToList();
                var oldCodes = db.WEB_OLD_CODE.Where(b => b.BIOTOPE_KEY == key).ToList();
                var speciesGrab = db.WEB_BIOT_SPECIES_GRAB.Where(b => b.BIOTOPE_KEY == key).ToList();
                var speciesObservation = db.WEB_BIOT_SPECIES_OBSERVATION.Where(b => b.BIOTOPE_KEY == key).ToList();
                var habitatCorrelations = db.WEB_HABITAT_CORRELATION.Where(b => b.BIOTOPE_KEY == key).ToList();
                var photos = db.WEB_PHOTO.Where(b => b.BIOTOPE_KEY == key).ToList();

                var transferObject = new
                {
                    Biotope = GetBiotopeDto(biotope[0]),
                    BiotopeHierarchy = GetBiotopeHierarchyDto(biotope[0].WEB_BIOTOPE_HIERARCHY),
                    SimilarBiotopes = GetSimilarBiotopesDto(similarBiotopes),
                    Species = GetCharacterisingSpeciesDto(speciesGrab, speciesObservation),
                    OldCodes = GetOldCodesDto(oldCodes),
                    HabitatCorrelations = GetHabitatCorrelationsDto(habitatCorrelations),
                    Photos = GetPhotosDto(photos)
                };

                return Json(transferObject, JsonRequestBehavior.AllowGet);
            }
        }

        private object GetBiotopeDto(WEB_BIOTOPE biotope)
        {
            var biotopeDto = new
            {
                BiotopeKey = biotope.BIOTOPE_KEY,
                OriginalCode = biotope.ORIGINAL_CODE,
                FullTerm = biotope.FULL_TERM,
                Description = biotope.DESCRIPTION,
                SpecialFeatures = biotope.SPECIAL_FEATURES,
                TemporalVariation = biotope.TEMPORAL_VARIATION,
                Situation = biotope.SITUATION,
                FrequencyKey = biotope.FREQUENCY_KEY,
                Frequency = biotope.FREQUENCY,
                Landscape = biotope.LANDSCAPE,
                Salinity = biotope.SALINITY,
                Height = biotope.HEIGHT,
                Exposure = biotope.EXPOSURE,
                TidalStreams = biotope.TIDAL_STREAMS,
                Substratum = biotope.SUBSTRATUM,
                Subzone = biotope.SUBZONE,
                SortCode = biotope.SORT_CODE,
                SensitivityAssessment = biotope.SENSITIVITY_ASSESSMENT,
                DerivedFrom = biotope.DERIVED_FROM,
                FaunalGroup = biotope.FAUNAL_GROUP
            };

            return biotopeDto;
        }

        private Dictionary<string, object> GetBiotopeHierarchyDto(IEnumerable<WEB_BIOTOPE_HIERARCHY> hierarchy)
        {
            var hierarchyDto = new Dictionary<string, object>();
            foreach (var currentBiotope in hierarchy)
            {
                hierarchyDto.Add(currentBiotope.HIGHERLEVEL.ToString(), new
                {
                    BiotopeKey = currentBiotope.WEB_BIOTOPE1.BIOTOPE_KEY,
                    OriginalCode = currentBiotope.WEB_BIOTOPE1.ORIGINAL_CODE
                });
            }
            return hierarchyDto;
        }

        private List<object> GetSimilarBiotopesDto(IEnumerable<WEB_BIOT_RELATION> similarBiotopes)
        {
            var similarBiotopesDto = new List<object>();
            foreach (var similarBiotope in similarBiotopes)
            {
                similarBiotopesDto.Add(new
                {
                    BiotopeKey = similarBiotope.WEB_BIOTOPE1.BIOTOPE_KEY,
                    OriginalCode = similarBiotope.WEB_BIOTOPE1.ORIGINAL_CODE,
                    Comment = similarBiotope.COMMENT
                });
            }

            return similarBiotopesDto;
        }

        private List<object> GetCharacterisingSpeciesDto(IEnumerable<WEB_BIOT_SPECIES_GRAB> speciesGrab,
            IEnumerable<WEB_BIOT_SPECIES_OBSERVATION> speciesObservation)
        {
            var characterisingSpecies = new List<object>();

            foreach (var species in speciesGrab)
            {
                characterisingSpecies.Add(new
                {
                    Name = species.ITEM_NAME,
                    Frequency = species.FREQ,
                    TypicalAbundance = species.ABUND,
                    SimilarityContribution = species.contrib_similarity_STRENGTH,
                    Abundance = species.SED_ABUND_SED_ABUND,
                    Sort = species.SORT
                });
            }

            foreach (var species in speciesObservation)
            {
                characterisingSpecies.Add(new
                {
                    Name = species.ITEM_NAME,
                    Frequency = species.FREQ,
                    TypicalAbundance = species.ABUND,
                    SimilarityContribution = species.contrib_similarity_STRENGTH,
                    Abundance = species.SED_ABUND_SED_ABUND,
                    Sort = species.SORT
                });
            }

            return characterisingSpecies;
        }

        private List<object> GetOldCodesDto(IEnumerable<WEB_OLD_CODE> oldCodes)
        {
            var oldCodesDto = new List<object>();
            foreach (var oldCode in oldCodes)
            {
                oldCodesDto.Add(new
                {
                    OriginalCode = oldCode.OLD_CODE,
                    Version = oldCode.VERSION,
                    PreviousFullname = oldCode.PREVIOUS_FULLNAME,
                    RelationshipType = oldCode.RELATIONSHIP_TYPE,
                    Modifications = oldCode.MODIFICATIONS
                });
            }

            return oldCodesDto;
        }

        private List<object> GetHabitatCorrelationsDto(IEnumerable<WEB_HABITAT_CORRELATION> habitatCorrelations)
        {
            var habitatCorrelationsDto = new List<object>();
            foreach (var habitat in habitatCorrelations)
            {
                habitatCorrelationsDto.Add(new
                {
                    RelatedClassificationSystem = habitat.RELATED_CLASSIFICATION_SYSTEM,
                    RelatedName = habitat.RELATED_NAME,
                    RelationshipType = habitat.RELATIONSHIP_TYPE,
                    ClassificationSystemUrl = habitat.CLASSIFICATION_SYSTEM_URL
                });
            }

            return habitatCorrelationsDto;
        }

        private List<object> GetPhotosDto(IEnumerable<WEB_PHOTO> photos)
        {
            var photosDto = new List<object>();
            foreach (var photo in photos)
            {
                photosDto.Add(new
                {
                    PhotoCaption = photo.PHOTO_CAPTION,
                    PhotoPath = photo.PHOTO_PATH
                });
            }

            return photosDto;
        }
    }
}