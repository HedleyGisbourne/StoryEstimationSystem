using System.Web.Mvc;
using HG.SoftwareEstimationService.Dto.ViewModels;
using HG.SoftwareEstimationService.Services.Contract;

namespace HG.SoftwareEstimationService.Web.Controllers
{
    public class StoriesController : ApplicationController
    {
        private readonly IStoriesService _storiesService;

        public StoriesController(IStoriesService storiesService)
        {
            _storiesService = storiesService;
        }

        public ActionResult Index(int id, bool allStories = false)
        {
            ViewBag.systemId = id;
            ViewBag.allStories = allStories 
                ? 1
                : 0;
            return View();
        }

        public ActionResult Observations(int systemId, int ticketId)
        {
            StoryGrid story = _storiesService.GetStory(ticketId);
            ViewBag.TicketName = story.TicketName;
            ViewBag.systemId = systemId;
            ViewBag.ticketId = ticketId;
            return View();
        }
    }
}