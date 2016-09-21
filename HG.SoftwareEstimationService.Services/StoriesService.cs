using System;
using System.Collections.Generic;
using System.Linq;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Dto.ViewModels;
using HG.SoftwareEstimationService.Repository;
using HG.SoftwareEstimationService.Services.Automapper;
using HG.SoftwareEstimationService.Services.Configuration;
using HG.SoftwareEstimationService.Services.Contract;
using HG.SoftwareEstimationService.SqliteData;

namespace HG.SoftwareEstimationService.Services
{
    public class StoriesService : IStoriesService
    {
        private readonly IRepositorySqlite<Story> _storyRepo;
        private readonly IEstimationService _estimationService;
        private readonly IObservationsService _observationsService;
        private readonly IDurationService _durationService;
        public StoriesService(
            IRepositorySqlite<Story> storyRepo,
            IEstimationService estimationService, 
            IObservationsService observationsService, 
            IDurationService durationService)
        {
            _storyRepo = storyRepo;
            _estimationService = estimationService;
            _observationsService = observationsService;
            _durationService = durationService;
        }

        public IEnumerable<StoryGrid> GetStories(int systemId, bool includeCompleted)
        {
            List<Story> stories = _storyRepo
                .GetMany(x => x.SystemId == systemId)
                .Where(x => includeCompleted || x.ActualCompletetionDuration == null)
                .ToList();

            return stories.Select(GetStoryGrid);
        }

        public StoryGrid GetStory(int storyId)
        {
            return GetStoryGrid(_storyRepo.GetById(storyId));
        }

        public string EstimateStory(int systemId, int storyId, EstimationConfig config = null)
        {
            if (!_observationsService.GetObservations(storyId).Any())
            {
                throw new ArgumentException("This story has no parts.");
            }

            return _estimationService.PerformEstimation(systemId, storyId, config);
        }

        public long AddStory(StoryGrid story)
        {
            Story s = AutomapperRegistrar.Map<Story>(story);
            _storyRepo.Add(s);
            _storyRepo.SaveChanges();

            return s.StoryId;
        }

        public long UpdateStory(StoryGrid story)
        {
            Story repoStory = _storyRepo.GetSingleOrDefault(sr => sr.StoryId == story.StoryId);
            repoStory.StoryTitle = story.StoryTitle;
            repoStory.TicketName = story.TicketName;

            _storyRepo.SaveChanges();

            return story.StoryId;
        }

        public void DeleteStory(long storyId)
        {
            _storyRepo.DeleteMany(x => x.StoryId == storyId);
            
            _storyRepo.SaveChanges();
        }

        public void CompleteStory(long storyId, DurationDto duration)
        {
            Story story = _storyRepo.GetSingleOrDefault(s => s.StoryId == storyId);

            story.ActualCompletetionDuration = _durationService.GetDuration(duration);
            story.CompletionDate = DateTime.Now;

            _storyRepo.SaveChanges();
        }

        private StoryGrid GetStoryGrid(Story story)
        {
            StoryGrid storyGrid = AutomapperRegistrar.Map<StoryGrid>(story);
            storyGrid.Observations = story.StoryParts.Count;
            storyGrid.ActualCompletionDuration = story.ActualCompletetionDuration == null
                ? "Incomplete"
                : _durationService.GetDuration((long)story.ActualCompletetionDuration.Value).ToString();
            storyGrid.EstimatedCompletionDuration = story.EstimatedCompletionDuration == null
                ? story.ActualCompletetionDuration == null
                    ? "Not yet estimated"
                    : "Was not estimated"
                : _durationService.GetDuration((long)story.EstimatedCompletionDuration.Value).ToString();
            storyGrid.CompletionDate = story.ActualCompletetionDuration != null && story.CompletionDate != null
                ? story.CompletionDate.Value.ToString("d MMM yyyy")
                : string.Empty;
            return storyGrid;
        }
    }
}