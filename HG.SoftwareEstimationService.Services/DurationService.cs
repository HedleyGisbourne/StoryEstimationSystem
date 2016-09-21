using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Services.Contract;

namespace HG.SoftwareEstimationService.Services
{
    public class DurationService : IDurationService
    {
        private readonly DurationDefinition _durationDefinition;

        public DurationService(IConfigurationService configurationService)
        {
            _durationDefinition = configurationService.GetDurationDefinition();
        }

        public DurationDto GetDuration(long quartersOfAnHour)
        {
            if (quartersOfAnHour < _durationDefinition.MinimumEstimation)
                quartersOfAnHour = _durationDefinition.MinimumEstimation;

            DurationDto duration = new DurationDto();
            duration.Years = quartersOfAnHour/_durationDefinition.QuarterHoursInYear;
            long remainder = quartersOfAnHour%_durationDefinition.QuarterHoursInYear;
            duration.Months = remainder/_durationDefinition.QuarterHoursInMonth;
            remainder %= _durationDefinition.QuarterHoursInMonth;
            duration.Weeks = remainder/_durationDefinition.QuarterHoursInWeek;
            remainder %= _durationDefinition.QuarterHoursInWeek;
            duration.Days = remainder/_durationDefinition.QuarterHoursInDay;
            remainder %= _durationDefinition.QuarterHoursInDay;
            duration.Hours = remainder/_durationDefinition.QuarterHoursInHour;
            remainder %= _durationDefinition.QuarterHoursInHour;
            duration.Minutes = remainder;
            return duration;
        }

        public long GetDuration(DurationDto durationDto)
        {
            long duration = 0;
            duration += durationDto.Years*_durationDefinition.QuarterHoursInYear;
            duration += durationDto.Months*_durationDefinition.QuarterHoursInMonth;
            duration += durationDto.Weeks*_durationDefinition.QuarterHoursInWeek;
            duration += durationDto.Days*_durationDefinition.QuarterHoursInDay;
            duration += durationDto.Hours*_durationDefinition.QuarterHoursInHour;
            duration += durationDto.Minutes/15;

            return duration;
        }
    }
}