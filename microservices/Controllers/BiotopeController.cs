using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using microservices.Models;

namespace microservices.Controllers
{
    public class BiotopeController : Controller
    {
        // GET: Biotope
        public ActionResult Index()
        {
            using (var db = new BiotopeDB())
            {
                var data = db.WEB_BIOTOPE_HIERARCHY.Select(b => new { b.BIOTOPE_KEY, b.BIOTOPE_PARENT_KEY }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Biotope/biotopeKey
        [Route("Biotope/{key}")]
        public ActionResult Index(string key)
        {
            using (var db = new BiotopeDB())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                var data = db.WEB_BIOTOPE.Where(b => b.BIOTOPE_KEY == key).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }
}