using HG.SoftwareEstimationService.Services.Configuration;

namespace HG.SoftwareEstimationService.Services.Contract
{
    public interface IEstimationService
    {
        string PerformEstimation(int systemId, int storyId, EstimationConfig config);
    }
}