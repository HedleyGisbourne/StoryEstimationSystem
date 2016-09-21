using System.Collections.Generic;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Dto.ViewModels;
using HG.SoftwareEstimationService.Services.Configuration;

namespace HG.SoftwareEstimationService.Services.Contract
{
    public interface IStoriesService
    {
        StoryGrid GetStory(int storyId);
        IEnumerable<StoryGrid> GetStories(int systemId, bool includeCompleted);
        string EstimateStory(int systemId, int storyId, EstimationConfig config = null);
        long AddStory(StoryGrid story);
        long UpdateStory(StoryGrid story);
        void DeleteStory(long storyId);
        void CompleteStory(long storyId, DurationDto duration);
    }
}