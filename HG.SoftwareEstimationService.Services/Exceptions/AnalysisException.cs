using System;

namespace HG.SoftwareEstimationService.Services.Exceptions
{
    [Serializable]
    public class AnalysisException : Exception
    {
        public AnalysisException(string message)
            : base(message)
        {
            
        }
    }
}
