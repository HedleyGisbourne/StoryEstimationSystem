using System.Linq;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Services.Exceptions;

namespace HG.SoftwareEstimationService.Services.Analysis.Contract
{
    /// <summary>
    /// Interface for Analysers. 
    /// If the analyser is a library the implementation should act as an adapter.
    /// </summary>
    public abstract class AnalyserBase
    {
        /// <summary>
        /// Performs analysis.
        /// </summary>
        /// <param name="trainingData">Training Data Dto.</param>
        /// <returns>Analysis Results Dto</returns>
        public AnalysisResults Perform(TrainingData trainingData)
        {
            if (trainingData.MatrixOfObsevsations.Any() == false)
            {
                throw new AnalysisException("There is no training data available.");
            }

            return PerformInternal(trainingData);
        }

        /// <summary>
        /// Implenetation of the analysis specific to concrete analyser.
        /// </summary>
        /// <param name="trainingData">Training Data Dto.</param>
        /// <returns>Analysis Results Dto</returns>
        protected abstract AnalysisResults PerformInternal(TrainingData trainingData);
    }
}