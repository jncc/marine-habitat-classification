using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace website.Models
{
    public class BiotopeModel : RenderModel
    {
        public BiotopeModel(IPublishedContent content) : base(content) {}

        public Biotope Biotope { get; set; }
        public List<Species> Species { get; set; }
        public Dictionary<int, BiotopeLevel> BiotopeHierarchy { get; set; }
        public List<SimilarBiotope> SimilarBiotopes { get; set; }
        public List<OldCode> OldCodes { get; set; }
    }

    public class Biotope
    {
        public string BiotopeKey { get; set; }
        public string OriginalCode { get; set; }
        public string FullTerm { get; set; }
        public string Description { get; set; }
        public string SpecialFeatures { get; set; }
        public string TemporalVariation { get; set; }
        public string Situation { get; set; }
        public string FrequencyKey { get; set; }
        public string Frequency { get; set; }
        public string Landscape { get; set; }
        public string Salinity { get; set; }
        public string Height { get; set; }
        public string Exposure { get; set; }
        public string TidalStreams { get; set; }
        public string Substratum { get; set; }
        public string Subzone { get; set; }
        public string SortCode { get; set; }
    }

    public class BiotopeLevel
    {
        public string BiotopeKey { get; set; }
        public string OriginalCode { get; set; }
    }

    public class SimilarBiotope
    {
        public string BiotopeKey { get; set; }
        public string OriginalCode { get; set; }
        public string Comment { get; set; }
    }

    public class Species
    {
        public string Name { get; set; }
        public string Frequency { get; set; }
        public string TypicalAbundance { get; set; }
        public short? SimilarityContribution { get; set; }
        public int? Abundance { get; set; }
        public string Sort { get; set; }
    }

    public class OldCode
    {
        public string OriginalCode { get; set; }
        public string Version { get; set; }
    }
}