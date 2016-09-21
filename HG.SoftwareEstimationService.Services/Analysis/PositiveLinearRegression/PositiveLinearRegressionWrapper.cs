using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Services.Analysis.Contract;
using HG.SoftwareEstimationService.Services.Exceptions;

namespace HG.SoftwareEstimationService.Services.Analysis.PositiveLinearRegression
{
    public class PositiveLinearRegressionWrapper : AnalyserBase
    {
        /// <summary>
        /// The Substitution string to insert the data into the Python script.
        /// </summary>
        private const string SubstutionString = "%%%%SUBSTRITUTIONSTRING%%%%%";

        protected override AnalysisResults PerformInternal(TrainingData trainingData)
        {
            string skeletonFile = PythonScripts.Analyser;
            string data = GetSubstrutionString(trainingData);
            string substituted = skeletonFile.Replace(SubstutionString, data);
            string fileName = string.Format(@"{0}\{1}.py", Path.GetTempPath(), Guid.NewGuid());
            string response;

            File.WriteAllText(fileName, substituted);

            try
            {
                response = CallPython(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem with configuration. Please check the path in the config is correct", ex);
            }
            finally
            {
                File.Delete(fileName);
            }

            return ParseResponse(response);
        }

        private AnalysisResults ParseResponse(string response)
        {
            string[] split = response.Replace(Environment.NewLine, string.Empty).Split(']').Select(x => x.Substring(1))
                .ToArray();

            return new AnalysisResults
            {
                Interceptor = double.Parse(split[1]),
                Coefficients = split[0].Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToList()
            };
        }

        private string GetSubstrutionString(TrainingData trainingData)
        {
            StringBuilder substution = new StringBuilder();

            substution.Append("[");
            bool addComma = false;
            foreach (IndependentVariables inputRow in trainingData.MatrixOfObsevsations)
            {
                if (addComma)
                {
                    substution.Append(", ");
                }
                else
                {
                    addComma = true;
                }

                substution.Append("[");
                substution.Append(string.Join(", ", inputRow));
                substution.Append("]");
            }
            
            substution.Append("], [");
            substution.Append(string.Join(", ", trainingData.IndependentVariables));
            substution.Append("]");

            return substution.ToString();
        }

        // http://stackoverflow.com/questions/32129336/can-scikit-learn-be-used-from-ironpython
        private static string CallPython(string cmd)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ConfigurationManager.AppSettings["PythonExecutablePath"],
                Arguments = string.Format("{0}", cmd),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            // Python requires Sklean, Scipy and Numpy packages available

            StringBuilder response = new StringBuilder();
            using (Process process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new AnalysisException("Problem starting analysis process. Please try again later.");
                }

                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    response.Append(result);
                }
            }

            return response.ToString();
        }
    }
}
