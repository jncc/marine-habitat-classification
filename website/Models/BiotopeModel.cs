using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace website.Models
{
    public class BiotopeModel : RenderModel
    {
        public Biotope Biotope { get; set; }

        public BiotopeModel(IPublishedContent content) : base(content) {}
    }
}