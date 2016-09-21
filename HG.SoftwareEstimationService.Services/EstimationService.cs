using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.Internal;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Repository;
using HG.SoftwareEstimationService.Services.Analysis.Contract;
using HG.SoftwareEstimationService.Services.Configuration;
using HG.SoftwareEstimationService.Services.Contract;
using HG.SoftwareEstimationService.Services.Exceptions;
using HG.SoftwareEstimationService.SqliteData;

namespace HG.SoftwareEstimationService.Services
{
    /// <summary>
    /// Service for estimating stories. 
    /// </summary>
    public class EstimationService : IEstimationService
    {
        private readonly IRepositorySqlite<PartType> _partTypeRepo;
        private readonly IRepositorySqlite<Story> _storyRepo;
        private readonly AnalyserBase _analyserBase;
        
        // Messages for user.
        private const string InsuffficientStoriesMessage =
            "There are only {0} stories which will be used as training data to estimate this task. " +
            "The estimation may be inaccurate.";
        private const string NoStoriesMessage =
            "There are no stories to use as training data. " +
            "Please complete some stories or change the eligibility configuration.";
        private const string InsuffficientTrainingDataMessage =
            "The estimation includes fields that do not yet have training data available. " +
            "Estimation cannot be performed for this ticket now.";

        /// <summary>
        /// Constructor for Estimation Service.
        /// </summary>
        /// <param name="partTypeRepo">Repository for Part Types.</param>
        /// <param name="storyRepo">Repository for Stories.</param>
        /// <param name="analyserFactory">Factory for AnalyserBase retrieval.</param>
        public EstimationService(
            IRepositorySqlite<PartType> partTypeRepo, 
            IRepositorySqlite<Story> storyRepo,
            IAnalyserFactory analyserFactory)
        {
            _partTypeRepo = partTypeRepo;
            _storyRepo = storyRepo;
            _analyserBase = analyserFactory.GetAnalyser();
        }

        /// <summary>
        /// Estimate a story using the training available completed stories flited by the rules in the Estimation config.
        /// </summary>
        /// <param name="systemId">Id of the system in development.</param>
        /// <param name="storyId">Id of the story being estimated.</param>
        /// <param name="config">Configuration for the which stories should be included in the training dataset.</param>
        /// <returns>Response message for user.</returns>
        public string PerformEstimation(int systemId, int storyId, EstimationConfig config)
        {
            Story storyToBeEstimated = _storyRepo.GetSingleOrDefault(s => s.StoryId == storyId);
            List<IndependentVariablesMapping> mappings = GetIndependentVariablesMappings();
            Story[] stories = GetAllStoriesForTrainingData(systemId, storyToBeEstimated, config);
            IndependentVariables independentVariablesForEstimation 
                = GetStoryIndependentVariables(storyToBeEstimated, mappings);
            
            string response = string.Empty;
            if (stories.Length < 6)
                response = string.Format(InsuffficientStoriesMessage, stories.Length);

            TrainingData trainingData = GetTrainingData(stories, mappings, independentVariablesForEstimation);
            AnalysisResults regressionResults = PerformStatisticalAnalysis(trainingData);
            PersistEstimation(regressionResults, independentVariablesForEstimation, storyToBeEstimated);

            return response;
        }

        /// <summary>
        /// Get Stories used in the training data. By default this will be all stories will a actual completion duration. 
        /// Stories can be filtered out using the EstimationConfiguration.
        /// </summary>
        /// <param name="systemId">Id of the system in development.</param>
        /// <param name="storyToBeEstimated">Entity for story to be estimated.</param>
        /// <param name="config">Configuration for the which stories should be included in the training dataset.</param>
        /// <returns>Array of stories</returns>
        private Story[] GetAllStoriesForTrainingData(int systemId, Story storyToBeEstimated, EstimationConfig config = null)
        {
            IEnumerable<Story> filteredStories = _storyRepo
                .GetMany(s => s.SystemId == systemId)
                .Where(s => s.StoryParts.Any())
                .Where(s => config == null || !config.ExcludeCurrentStory || s != storyToBeEstimated)
                .Where(s => config == null || !config.ExcludeUnestimatedStories || s.EstimatedCompletionDuration != null)
                .Where(s => s.ActualCompletetionDuration != null);

            if (config != null && config.GetAdditionalFilters().Any())
            {
                filteredStories = config.GetAdditionalFilters()
                    .Aggregate(filteredStories, (current, filter) => current.Where(filter))
                    .ToArray();
            }

            Story[] stories = config != null && config.GetStorySpecificFilters().Any()
                ? config.GetStorySpecificFilters()
                    .Aggregate(filteredStories, (current, filter) => current.Where(s => filter(s, storyToBeEstimated)))
                    .ToArray()
                : filteredStories.ToArray();

            if (!stories.Any())
            {
                throw new EstimationException(NoStoriesMessage);
            }

            return stories;
        }

        /// <summary>
        /// Get the training data for the analysis.
        /// </summary>
        /// <param name="stories">Stories in the training dataset.</param>
        /// <param name="mappings">Independent variable mappings.</param>
        /// <param name="storyToEstimateIndependentVariables">Independent variables of the story to be estimated.</param>
        /// <returns>Traing data for the analysis.</returns>
        private TrainingData GetTrainingData(
            Story[] stories,
            List<IndependentVariablesMapping> mappings,
            IndependentVariables storyToEstimateIndependentVariables)
        {
            TrainingData trainingData = new TrainingData();

            stories.Each(story =>
            {
                trainingData.MatrixOfObsevsations.Add(GetStoryIndependentVariables(story, mappings));
                if (story.ActualCompletetionDuration == null)
                    throw new AnalysisException("Attempt to use story without completion duration in training data set.");
                trainingData.IndependentVariables.Add(story.ActualCompletetionDuration.Value);
            });

            FindAndRemoveRedundantVariables(trainingData, storyToEstimateIndependentVariables);
            AddDummyObsevation(trainingData);

            return trainingData;
        }

        private void PersistEstimation(
            AnalysisResults analysisResults, 
            IndependentVariables independentVariablesForEstimation, 
            Story story)
        {
            double result = analysisResults.Interceptor + analysisResults.Coefficients
                .Select((coefficient, i) => coefficient * independentVariablesForEstimation[i]).Sum();

            story.EstimatedCompletionDuration = Math.Ceiling(result);
            _storyRepo.SaveChanges();
        }

        /// <summary>
        /// Retrieve how the story parts and properties will map to a tuple of independent variables.
        /// </summary>
        /// <returns>Mappings.</returns>
        private List<IndependentVariablesMapping> GetIndependentVariablesMappings()
        {
            PartType[] partTypes = _partTypeRepo.GetAll().ToArray();

            List<IndependentVariablesMapping> mappings = new List<IndependentVariablesMapping>();
            int i = 0;

            foreach (PartType partType in partTypes)
            {
                mappings.Add(new IndependentVariablesMapping(partType, RecordType.PartType, i));
                i++;

                foreach (PartTypeProperty property in partType.PartTypeProperties)
                {
                    if (property.Property.EnumerationId != null) // Nominal variable.
                    {
                        foreach (EnumerationItem enumueration in property.Property.Enumeration.EnumerationItems)
                        {
                            // Include a bool for each nominal category.
                            IndependentVariablesMapping mapping = new IndependentVariablesMapping(
                                partType, 
                                RecordType.Nominal, 
                                i, 
                                property, 
                                enumueration.EnumerationEnumerationId);
                            mappings.Add(mapping);
                            i++;
                        }
                    }
                    else // Interval variable.
                    {
                        // Add dummy variable for interval types.
                        mappings.Add(new IndependentVariablesMapping(partType, RecordType.Bool, i, property)); 
                        i++;
                        mappings.Add(new IndependentVariablesMapping(partType, RecordType.Interval, i, property));
                        i++;
                    }
                }
            }

            return mappings;
        }

        /// <summary>
        /// Find and remove all independent variables that are 0 in all observations and independent variables to estimate.
        /// </summary>
        /// <param name="trainingData">Training data.</param>
        /// <param name="independentVariablesForEstimation">Independent variables for estimation.</param>
        private void FindAndRemoveRedundantVariables(
            TrainingData trainingData, 
            IndependentVariables independentVariablesForEstimation)
        {
            RedundantVariableIndices redundantVariableIndices = new RedundantVariableIndices();

            for (int i = 0; i < trainingData.MatrixOfObsevsations[0].Count; i++)
            {
                if (trainingData.MatrixOfObsevsations.Any(observation => observation[i] != 0)) 
                    continue;

                if (independentVariablesForEstimation[i] != 0)
                    throw new EstimationException(InsuffficientTrainingDataMessage);

                redundantVariableIndices.Add(i);
            }

            RemoveRedundantVariables(independentVariablesForEstimation, redundantVariableIndices);
            RemoveRedundantVariables(trainingData, redundantVariableIndices);
        }

        private void RemoveRedundantVariables(TrainingData trainingData, RedundantVariableIndices redundantVariableIndices)
        {
            foreach (IndependentVariables observation in trainingData.MatrixOfObsevsations)
            {
                RemoveRedundantVariables(observation, redundantVariableIndices);
            }
        }

        /// <summary>
        /// Remove redundant independent variables.
        /// </summary>
        /// <param name="independentVariables">Independent variables to have redundant variables removed.</param>
        /// <param name="redundantVariableIndices">The indices of redundant variables.</param>
        private void RemoveRedundantVariables(
            IndependentVariables independentVariables, 
            RedundantVariableIndices redundantVariableIndices)
        {
            foreach (int redundantVariableIndex in redundantVariableIndices)
            {
                independentVariables.RemoveAt(redundantVariableIndex);
            }
        }

        /// <summary>
        /// Get the independent variables for story.
        /// </summary>
        /// <param name="story">Story for which the independent variable should be retrived.</param>
        /// <param name="mapppings">Mappings.</param>
        /// <returns>Independent variables.</returns>
        private IndependentVariables GetStoryIndependentVariables(Story story, List<IndependentVariablesMapping> mapppings)
        {
            IndependentVariables independentVariables = new IndependentVariables(mapppings.Count);
            foreach (StoryPart storyPart in story.StoryParts)
            {
                GetPartIndependentVariables(mapppings, storyPart, independentVariables);
            }

            return independentVariables;
        }

        /// <summary>
        /// Accumulate the indenpendent variable for a story part.
        /// </summary>
        /// <param name="mappings">Mappings.</param>
        /// <param name="storyPart">Part of story for which variables should be accumulated.</param>
        /// <param name="independentVariablesForEstimation">Current state of independent variables for story.</param>
        private static void GetPartIndependentVariables(
            List<IndependentVariablesMapping> mappings, 
            StoryPart storyPart, 
            IndependentVariables independentVariablesForEstimation)
        {
            if (storyPart.PartTypeId != null)
            {
                int partTypeIndex = mappings
                    .Single(d => d.PartTypeId == storyPart.PartTypeId && d.RecordType == RecordType.PartType).Index;
                independentVariablesForEstimation[partTypeIndex] = independentVariablesForEstimation[partTypeIndex] + 1;
            }

            foreach (StoryPartProperty storyPartProperty in storyPart.StoryPartProperties)
            {
                if (storyPartProperty.Property.EnumerationId != null)
                {
                    IndependentVariablesMapping mapping = mappings.Single(d =>
                        d.PropertyId == storyPartProperty.PropertyId
                        && d.PartTypeId == storyPartProperty.StoryPart.PartTypeId
                        && d.EnumId == storyPartProperty.PropertyValue);

                    independentVariablesForEstimation[mapping.Index] = independentVariablesForEstimation[mapping.Index] + 1;
                }
                else if (storyPartProperty.PropertyValue != null && storyPartProperty.PropertyValue > 0)
                {
                    IndependentVariablesMapping mapping = mappings.Single(d =>
                        d.PropertyId == storyPartProperty.PropertyId
                        && d.PartTypeId == storyPartProperty.StoryPart.PartTypeId
                        && d.RecordType == RecordType.Bool);
                    independentVariablesForEstimation[mapping.Index] = independentVariablesForEstimation[mapping.Index] + 1;
                    independentVariablesForEstimation[mapping.Index + 1] 
                        = independentVariablesForEstimation[mapping.Index] + (int)storyPartProperty.PropertyValue.Value;
                }
            }
        }

        /// <summary>
        /// Add a dummy observations of all 0.
        /// This states that an observation with all 0 independent variables would have a 0 dependent variable.
        /// </summary>
        /// <param name="trainingData">Traing data.</param>
        private static void AddDummyObsevation(TrainingData trainingData)
        {
            trainingData.IndependentVariables.Add(0);
            trainingData.MatrixOfObsevsations.Add(new IndependentVariables(trainingData.MatrixOfObsevsations[0].Count));
        }

        /// <summary>
        /// Performs analysis by calling the implementation of AnalyserBase.
        /// </summary>
        /// <param name="trainingData">Training data.</param>
        /// <returns>Analysis results.</returns>
        private AnalysisResults PerformStatisticalAnalysis(TrainingData trainingData)
        {
            AnalysisResults regressionResults = _analyserBase.Perform(trainingData);
            return regressionResults;
        }

        /// <summary>
        /// Mapping from part, property and property type to the independent variable tuple index.
        /// </summary>
        private class IndependentVariablesMapping
        {
            public long PartTypeId { get; private set; }
            public long? PropertyId { get; private set; }
            public long? EnumId { get; private set; }
            public RecordType RecordType { get; private set; }
            public int Index { get; private set; }

            public IndependentVariablesMapping(
                PartType partTypeId, 
                RecordType recordType, 
                int column, 
                PartTypeProperty property = null, 
                long? enumId = null)
            {
                PropertyId = property == null ? (long?)null : property.PropertyId;
                PartTypeId = partTypeId.PartTypeId;
                RecordType = recordType;
                Index = column;
                EnumId = enumId;
            }
        }

        /// <summary>
        /// Store of independent variables that are 0 in all observations and independent variables to estimate. 
        /// Enforces indcies in decending order so that can be removed safely.
        /// </summary>
        private class RedundantVariableIndices : IEnumerable<int>
        {
            private readonly List<int>  _indices = new List<int>();
            private IEnumerable<int> _descendingIndices;

            public void Add(int index)
            {
                _indices.Add(index);
                _descendingIndices = _indices.OrderByDescending(x => x);
            }

            public IEnumerator<int> GetEnumerator()
            {
                return _descendingIndices.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        
        /// <summary>
        /// Mapping type for conversion to independent variable.
        /// </summary>
        private enum RecordType
        {
            Nominal,
            Interval,
            Bool,
            PartType
        }

    }
}