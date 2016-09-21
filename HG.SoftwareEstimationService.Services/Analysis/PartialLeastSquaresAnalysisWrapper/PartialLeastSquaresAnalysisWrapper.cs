using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Services.Analysis.Contract;

namespace HG.SoftwareEstimationService.Services.Analysis.PartialLeastSquaresAnalysisWrapper
{
    public class PartialLeastSquaresAnalysisWrapper : AnalyserBase
    {
        protected override AnalysisResults PerformInternal(TrainingData trainingData)
        {
            int rows = trainingData.MatrixOfObsevsations.Count;
            int cols = trainingData.MatrixOfObsevsations[0].Count;
            double[,] formattedInputs = new double[rows, cols];
            double[,] formattedOutputs = new double[rows, 1];

            int i = 0;
            foreach (IndependentVariables story in trainingData.MatrixOfObsevsations)
            {
                int ii = 0;
                foreach (int filed in story)
                {
                    formattedInputs[i, ii] = filed;
                    ii++;
                }

                formattedOutputs[i, 0] = trainingData.IndependentVariables[i];

                i++;
            }

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                IndependentVariables row = trainingData.MatrixOfObsevsations[rowIndex];
                for (int colIndex = 0; colIndex < cols; colIndex++)
                {
                    formattedInputs[rowIndex, colIndex] = row[colIndex];
                }
            }
            
            for (int outputRowIndex = 0; outputRowIndex < rows; outputRowIndex++)
            {
                double output = trainingData.IndependentVariables[outputRowIndex];
                formattedOutputs[outputRowIndex, 0] = output;
            }

            PartialLeastSquaresAnalysis pls = new PartialLeastSquaresAnalysis(
                formattedInputs,
                formattedOutputs,
                AnalysisMethod.Standardize,
                PartialLeastSquaresAlgorithm.SIMPLS);
            pls.Compute();

            MultivariateLinearRegression regressionResults = pls.CreateRegression();

            AnalysisResults analysisResults = new AnalysisResults
            {
                Interceptor = regressionResults.Intercepts[0],
            };

            for (i = 0; i < regressionResults.Coefficients.GetUpperBound(0) + 1 ;i++)
            {
                analysisResults.Coefficients.Add(regressionResults.Coefficients[i, 0]);
            }

            return analysisResults;
        }
    }
}