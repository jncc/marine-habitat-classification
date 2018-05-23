using System.Web.Routing;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace MHC.Umbraco.Plugin.App_Start
{
    public class BiotopeRouteHandler : UmbracoVirtualNodeRouteHandler
    {
        protected override IPublishedContent FindContent(RequestContext requestContext, UmbracoContext umbracoContext)
        {
            return umbracoContext.ContentCache.GetByRoute("/biotopes/biotope");
        }
    }
}