using System.Text;

namespace HG.SoftwareEstimationService.Dto
{
    public class DurationDto
    {
        public long Minutes { get; set; }
        public long Hours { get; set; }
        public long Days { get; set; }
        public long Weeks { get; set; }
        public long Months { get; set; }
        public long Years { get; set; }


        public string ToVerboseString()
        {
            return GetString(true);
        }

        public override string ToString()
        {
            return GetString(false);
        }

        private string GetString(bool verbose)
        {
            StringBuilder output = new StringBuilder();

            if (Years > 0)
                output.Append(string.Format("{0}{1} ", Years, verbose ? "Years" : "Y"));
            if (Months > 0)
                output.Append(string.Format("{0}{1} ", Months, verbose ? "Months" : "M"));
            if (Weeks > 0)
                output.Append(string.Format("{0}{1} ", Weeks, verbose ? "Weeks" : "W"));
            if (Days > 0)
                output.Append(string.Format("{0}{1} ", Days, verbose ? "Days" : "D"));
            if (Hours > 0)
                output.Append(string.Format("{0}{1} ", Hours, verbose ? "Hours" : "h"));
            if (Minutes > 0)
                output.Append(string.Format("{0}{1} ", Minutes * 15, verbose ? "Minutes" : "m"));

            return output.ToString();
        }
    }
}
