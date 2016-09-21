using System.Configuration;
using HG.SoftwareEstimationService.Services.Analysis.Contract;
using HG.SoftwareEstimationService.Services.Analysis.PositiveLinearRegression;

namespace HG.SoftwareEstimationService.Services.Analysis
{
    public class AnalysierFactory : IAnalyserFactory
    {
        public AnalyserBase GetAnalyser()
        {
            string analyser = ConfigurationManager.AppSettings["AnalyserType"];

            switch (analyser)
            {
                case "PositiveLinearRegression":
                    return new PositiveLinearRegressionWrapper();
                case "PartialLeastSquaresAnalysis":
                    return new PartialLeastSquaresAnalysisWrapper.PartialLeastSquaresAnalysisWrapper();
                default:
                    throw new ConfigurationErrorsException("AnalyserType is not configured correctly.");
            }
        }
    }
}
