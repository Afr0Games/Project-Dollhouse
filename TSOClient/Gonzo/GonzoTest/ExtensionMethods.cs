using System;

namespace GonzoTest
{
    public static class ExtensionMethods
    {
        public static bool IsSameDay(this DateTime Datetime1, DateTime Datetime2)
        {
            return Datetime1.Year == Datetime2.Year
                && Datetime1.Month == Datetime2.Month
                && Datetime1.Day == Datetime2.Day;
        }
    }
}
