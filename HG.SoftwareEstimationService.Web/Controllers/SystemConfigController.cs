using System.Web.Http;
using System.Web.Mvc;

namespace HG.SoftwareEstimationService.Web.Controllers
{
    public class SystemConfigController : Controller
    {
        // GET: SystemConfig/{systemsId}
        public ActionResult Index([FromUri]int systemId)
        {
            // TODO check the systemId exists
            return View(systemId);
        }

        // GET: SystemConfig/{systemsId}/Layers
        public ActionResult Layers([FromUri]int systemId)
        {
            // TODO check the systemId exists
            return View(systemId);
        }

        // GET: SystemConfig/{systemsId}/Subsystems
        public ActionResult Subsystems([FromUri]int systemId)
        {
            // TODO check the systemId exists
            return View(systemId);
        }

        // GET: SystemConfig/{systemsId}/Enumerations
        public ActionResult Enumerations([FromUri]int systemId)
        {
            // TODO check the systemId exists
            return View(systemId);
        }

        // GET: SystemConfig/{systemsId}/ComplexityComponents
        public ActionResult ComplexityComponents([FromUri]int systemId)
        {
            // TODO check the systemId exists
            return View(systemId);
        }
    }
}