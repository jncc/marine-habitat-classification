using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace website.Models
{
    public class BiotopeModel : RenderModel
    {
        public BiotopeModel(IPublishedContent content) : base(content) {}

        public WEB_BIOTOPE Biotope { get; set; }
//        public List<WEB_BIOT_SPECIES> Species { get; set; }
        public List<WEB_BIOT_RELATION> SimilarBiotopes { get; set; }
        public List<WEB_OLD_CODE> OldCodes { get; set; }
    }
}