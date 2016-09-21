using HG.SoftwareEstimationService.Dto;

namespace HG.SoftwareEstimationService.Services.Contract
{
    public interface IConfigurationService
    {
        DurationDefinition GetDurationDefinition();
    }
}