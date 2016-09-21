using System.Web.Http;
using System.Web.Mvc;
using HG.SoftwareEstimationService.Dto;

namespace HG.SoftwareEstimationService.Web.Controllers
{
    public class ModalController : ApplicationController
    {
        [System.Web.Mvc.HttpPost]
        public ActionResult Index([FromBody]ModalModel modal)
        {
            return View(modal);
        }        
    }
}