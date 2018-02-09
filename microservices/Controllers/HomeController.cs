using microservices.Models;
using System.Linq;
using System.Web.Mvc;

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
