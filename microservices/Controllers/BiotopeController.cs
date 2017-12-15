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
        public JsonResult Index()
        {
            using (var db = new BiotopeDB())
            {
                var data = db.WEB_BIOTOPE_HIERARCHY.Select(b => new { b.BIOTOPE_KEY, b.BIOTOPE_PARENT_KEY }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Biotope/biotopeKey
        [Route("Biotope/{key}")]
        public JsonResult Index(string key)
        {
            using (var db = new BiotopeDB())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                var biotope = db.WEB_BIOTOPE.Where(b => b.BIOTOPE_KEY == key).ToList();
                var transferObject = new
                {
                    Biotope = biotope,
//                    Species = GetCharacterisingSpecies(key),
                    SimilarBiotopes = GetSimilarBiotopes(key),
                    OldCodes = GetOldCodes(key),
                };
                return Json(transferObject, JsonRequestBehavior.AllowGet);
            }
        }

//        private List<WEB_BIOT_SPECIES> GetCharacterisingSpecies(string key)
//        {
//            using (var db = new BiotopeDB())
//            {
//                db.Configuration.LazyLoadingEnabled = false;
//                db.Configuration.ProxyCreationEnabled = false;
//
//                return db.WEB_BIOT_SPECIES.Where(c => c.BIOTOPE_KEY == key).ToList();
//            }
//        }

        private List<WEB_BIOT_RELATION> GetSimilarBiotopes(string key)
        {
            using (var db = new BiotopeDB())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                return db.WEB_BIOT_RELATION.Where(c => c.BIOTOPE_KEY == key).ToList();
            }
        }

        private List<WEB_OLD_CODE> GetOldCodes(string key)
        {
            using (var db = new BiotopeDB())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                return db.WEB_OLD_CODE.Where(c => c.BIOTOPE_KEY == key).ToList();
            }
        }
    }
}