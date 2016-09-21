using HG.SoftwareEstimationService.Dto;

namespace HG.SoftwareEstimationService.Services.ApplicationConfiguration.Contract
{
    public interface IApplicationConfigurationService
    {
        ApplicationConfigurationDto GetApplicationConfiguration();
    }
}