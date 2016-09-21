using HG.SoftwareEstimationService.Dto;

namespace HG.SoftwareEstimationService.Services.Contract
{
    public interface IDurationService
    {
        DurationDto GetDuration(long quartersOfAnHour);
        long GetDuration(DurationDto duration);
    }
}