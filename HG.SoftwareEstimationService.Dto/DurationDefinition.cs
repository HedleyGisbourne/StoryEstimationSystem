namespace HG.SoftwareEstimationService.Dto
{
    public class DurationDefinition
    {
        public long MinimumEstimation { get; set; }
        public long MaximumEstimation { get; set; }

        public long HoursInWorkDay { get; set; }
        public long DaysInWorkWeek { get; set; }
        public long DaysInWorkMonth { get; set; }
        public long WeeksInWorkYear { get; set; }

        public long QuarterHoursInHour { get { return 4; } }

        public long QuarterHoursInDay
        {
            get { return QuarterHoursInHour * HoursInWorkDay; }
        }

        public long QuarterHoursInWeek
        {
            get { return QuarterHoursInDay * DaysInWorkWeek; }
        }

        public long QuarterHoursInMonth
        {
            get { return QuarterHoursInWeek * DaysInWorkMonth; }
        }

        public long QuarterHoursInYear
        {
            get { return WeeksInWorkYear * QuarterHoursInWeek; }
        }
    }
}
