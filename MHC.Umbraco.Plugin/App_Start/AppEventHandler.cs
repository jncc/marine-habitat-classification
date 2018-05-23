using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web;

namespace MHC.Umbraco.Plugin.App_Start
{
    public class AppEventHandler : ApplicationEventHandler
    {

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            RouteTable.Routes.MapUmbracoRoute(
                "Biotope",
                "biotopes/biotope/{key}",
                new {controller = "Biotope", action = "Biotope", key = UrlParameter.Optional},
                new BiotopeRouteHandler(),
                namespaces: new[] { "MHC.Umbraco.Plugin.Controllers" }
            );
        }
    }
}