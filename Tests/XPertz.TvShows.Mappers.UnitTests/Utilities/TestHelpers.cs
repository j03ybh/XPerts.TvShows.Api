using System;

namespace XPertz.TvShows.Mappers.UnitTests.Utilities
{
    internal static class TestHelpers
    {
        internal static bool AreMappedDateTimesEqual(object dateTimeObject, DateTime dateTime)
        {
            if (dateTimeObject is DateTime dateValue)
            {
                return dateValue.Year == dateTime.Year
                    && dateValue.Month == dateTime.Month
                    && dateValue.Day == dateTime.Day
                    && dateValue.Hour == dateTime.Hour
                    && dateValue.Minute == dateTime.Minute
                    && dateValue.Second == dateTime.Second;
            }

            return false;
        }

        internal static bool AreMappedDateTimesEqual(DateTime dateTimeObject, DateTime dateTime)
        {
            return dateTimeObject.Year == dateTime.Year
                && dateTimeObject.Month == dateTime.Month
                && dateTimeObject.Day == dateTime.Day
                && dateTimeObject.Hour == dateTime.Hour
                && dateTimeObject.Minute == dateTime.Minute
                && dateTimeObject.Second == dateTime.Second;
        }
    }
}