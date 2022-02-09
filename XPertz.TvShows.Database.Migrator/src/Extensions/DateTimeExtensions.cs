namespace XPertz.TvShows.Database.Migrator.Extensions
{
    internal static class DateTimeExtensions
    {
        internal static bool IsDateEqualTo(this DateTime date, DateTime other)
        {
            return date.Year == other.Year && date.Month == other.Month && date.Day == other.Day;
        }
    }
}