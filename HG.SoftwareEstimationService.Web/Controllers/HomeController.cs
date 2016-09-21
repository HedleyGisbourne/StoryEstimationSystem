using System.Web.Mvc;

namespace HG.SoftwareEstimationService.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}