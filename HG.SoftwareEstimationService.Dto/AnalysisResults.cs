using System.Collections.Generic;

namespace HG.SoftwareEstimationService.Dto
{
    public class AnalysisResults
    {
        public AnalysisResults()
        {
            Coefficients = new List<double>();
        }

        public List<double> Coefficients { get; set; }
        public double Interceptor { get; set; }
    }
}
