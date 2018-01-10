using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                db.Configuration.LazyLoadingEnabled = true;
                db.Configuration.ProxyCreationEnabled = true;

                var biotope = db.WEB_BIOTOPE.Where(b => b.BIOTOPE_KEY == key).ToList();
                var similarBiotopes = db.WEB_BIOT_RELATION.Where(c => c.BIOTOPE_KEY == key).ToList();
                var oldCodes = db.WEB_OLD_CODE.Where(c => c.BIOTOPE_KEY == key).ToList();

                var hierarchy = new Dictionary<string, string>();

                foreach (var currentBiotope in biotope[0].WEB_BIOTOPE_HIERARCHY)
                {
                    hierarchy.Add(currentBiotope.HIGHERLEVEL.ToString(), currentBiotope.WEB_BIOTOPE1.ORIGINAL_CODE);
                }

                var transferObject = new
                {
//                    Biotope = biotope,
                    BiotopeHierarchy = hierarchy,
//                    Species = GetCharacterisingSpecies(key),
//                    SimilarBiotopes = similarBiotopes,
//                    OldCodes = oldCodes
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

//        private List<WEB_BIOT_RELATION> GetSimilarBiotopes(BiotopeDB db, string key)
//        {
////            db.WEB_BIOT_RELATION.Include(m => m.WEB_BIOTOPE);
//
//            return 
//        }
//
//        private List<WEB_OLD_CODE> GetOldCodes(BiotopeDB db, string key)
//        {
//            return db.WEB_OLD_CODE.Where(c => c.BIOTOPE_KEY == key).ToList();
//        }
    }
}