namespace HG.SoftwareEstimationService.Helpers
{
    public static class StringExtensions
    {
        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}
