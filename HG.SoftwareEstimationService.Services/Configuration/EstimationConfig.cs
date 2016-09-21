using System;
using System.Collections.Generic;
using HG.SoftwareEstimationService.SqliteData;

namespace HG.SoftwareEstimationService.Services.Configuration
{
    /// <summary>
    /// Configuration for the analyser.
    /// Filters can be applied 
    /// </summary>
    public class EstimationConfig
    {
        private readonly List<Func<Story, bool>> _additionalFilters;
        private readonly List<Func<Story, Story, bool>> _additionalStorySpecificFilters;
        
        public EstimationConfig()
        {
            _additionalFilters = new List<Func<Story, bool>>();
            _additionalStorySpecificFilters = new List<Func<Story, Story, bool>>();
        }

        public bool ExcludeCurrentStory { get; set; }
        public bool ExcludeUnestimatedStories { get; set; }

        public void AddAdditionalFilter(Func<Story, bool> filter)
        {
            _additionalFilters.Add(filter);
        }

        public List<Func<Story, bool>> GetAdditionalFilters()
        {
            return _additionalFilters;
        }

        public void AddStorySpecificFilters(Func<Story, Story, bool> filter)
        {
            _additionalStorySpecificFilters.Add(filter);
        }

        public List<Func<Story, Story, bool>> GetStorySpecificFilters()
        {
            return _additionalStorySpecificFilters;
        } 
    }
}
