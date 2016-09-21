using HG.SoftwareEstimationService.Dto.ViewModels;
using HG.SoftwareEstimationService.Web.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using HG.SoftwareEstimationService.CompositionRoot;
using HG.SoftwareEstimationService.Services.Configuration;
using HG.SoftwareEstimationService.Services.Contract;

namespace EstimateAllCompletedStories
{
    /// <summary>
    /// This will be used to estimate all of the training data using all of the training data. This is necessary to compare it to other models
    /// which would work in the same way like the excel model. All training data needs a completetion time.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            HelperService helperService = new HelperService();
            helperService.EstimateAllCompleteStories();
        }
    }

    public class HelperService
    {
        private const int SystemId = 0;

        public HelperService()
        {
            WinsorInstaller controllerInstaller = new WinsorInstaller();
            CompositionRoot.InstallAsembly(controllerInstaller);
        }

        public void EstimateAllCompleteStories()
        {
            IStoriesService service = CompositionRoot.Resolve<IStoriesService>();
            IEnumerable<StoryGrid> stories = service.GetStories(SystemId, true);

            EstimationConfig config = new EstimationConfig
            {
                ExcludeCurrentStory = false,
                ExcludeUnestimatedStories = false
            };

            // Estimation of training data
            //   config.AddAdditionalFilter(x => x.TicketName.StartsWith("X"));
            // StoryGrid[] eligibleStories =
            //    stories.Where(x => x.EstimatedCompletionDuration == "Was not estimated" && x.TicketName.StartsWith("X"))
            //    .ToArray();
            // foreach (var storyGrid in eligibleStories)
            // End of section

            // Estimation of test data
            config.AddStorySpecificFilters((x, y) => x.CompletionDate < y.CompletionDate);
            foreach (StoryGrid storyGrid in stories.Where(x => x.EstimatedCompletionDuration == "Was not estimated"))
            // End of section
            {
                try
                {
                    service.EstimateStory(SystemId, storyGrid.StoryId, config);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}