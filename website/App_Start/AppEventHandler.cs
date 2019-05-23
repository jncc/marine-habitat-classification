using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web;

namespace website.App_Start
{
    public class AppEventHandler : ApplicationEventHandler
    {

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            RouteTable.Routes.MapUmbracoRoute(
                "Biotopes",
                "biotopes/{key}",
                new {controller = "Biotopes", action = "Biotopes", key = UrlParameter.Optional},
                new BiotopeRouteHandler()
            );
        }
    }
}