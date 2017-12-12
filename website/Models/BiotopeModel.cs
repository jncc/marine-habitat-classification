using System.Globalization;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace website.Models
{
    public class BiotopeModel : RenderModel
    {
        public BiotopeModel(IPublishedContent content) : base(content)
        {
        }

        public string Key { get; set; }
        public string Description { get; set; }
    }
}