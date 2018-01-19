using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace website.Controllers
{
    public static class ErrorHandlingExtension
    {
        public static IPublishedContent GetErrorPage(this UmbracoHelper h)
        {
            var errorPage = new umbraco.handle404();
            //providing the URL logs a sensible error message
            errorPage.Execute(h.UmbracoContext.HttpContext.Request.Url.PathAndQuery);
            return h.TypedContent(errorPage.redirectID);
        }
    }
}