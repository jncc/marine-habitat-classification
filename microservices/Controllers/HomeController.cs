using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using microservices.Models;

namespace microservices.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new BiotopeDB())
            {
                var biotopeCount = db.WEB_BIOTOPE.Count();
                var version = typeof(WebApiApplication).Assembly.GetName().Version;
                return Json(new { biotopeCount, version }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
