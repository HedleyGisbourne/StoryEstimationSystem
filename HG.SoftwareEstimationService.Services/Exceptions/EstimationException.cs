using System;

namespace HG.SoftwareEstimationService.Services.Exceptions
{
    [Serializable]
    public class EstimationException : Exception
    {
        public EstimationException(string message)
            : base(message)
        {
        }
    }
}