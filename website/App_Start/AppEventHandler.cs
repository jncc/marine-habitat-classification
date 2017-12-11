using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace website.App_Start
{
    public class AppEventHandler : ApplicationEventHandler
    {

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            RouteTable.Routes.MapMvcAttributeRoutes();

            RouteTable.Routes.MapRoute(
                name: "Biotope",
                url: "marine-habitat-classification/biotopes/biotope/{key}",
                defaults: new {controller = "Biotope", action = "Biotope", key = UrlParameter.Optional}
            );
        }
    }
}