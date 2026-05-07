using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Shared.Extensions
{
    public static class DateTimeExtensions
    {
        //public static DateTime ToClient(this DateTime dateTime) => TimeZoneInfo.ConvertTimeFromUtc(dateTime.KindOfUtc(), Values.TimeZone.Client).RoundToNearestSecond();
        //public static DateTime ToClientExact(this DateTime dateTime) => TimeZoneInfo.ConvertTimeFromUtc(dateTime.KindOfUtc(), Values.TimeZone.Client);
        //public static DateTime FromClient(this DateTime dateTime) => TimeZoneInfo.ConvertTimeToUtc(dateTime.KindOf(DateTimeKind.Unspecified), Values.TimeZone.Client).RoundToNearestSecond();

        public static DateTime KindOf(this DateTime dateTime, DateTimeKind kind) => DateTime.SpecifyKind(dateTime, kind);
        public static DateTime KindOfUtc(this DateTime dateTime) => dateTime.KindOf(DateTimeKind.Utc);
        public static DateTime KindOfUnspecified(this DateTime dateTime) => dateTime.KindOf(DateTimeKind.Unspecified);

        public static string ToIso8601(this DateTime dateTime) => dateTime.ToString("o");

        public static DateTime ToDateTime(this long value) => DateTimeOffset.FromUnixTimeMilliseconds(value).UtcDateTime;

        public static DateTime RoundToNearestSecond(this DateTime dateTime)
        {
            long ticks = dateTime.Ticks;
            long secondTicks = TimeSpan.TicksPerSecond;

            // Добавляем половину интервала для математического округления
            long roundedTicks = (ticks + secondTicks / 2) / secondTicks * secondTicks;

            return new DateTime(roundedTicks, dateTime.Kind);
        }

        public static bool IsStSn(this DateTime dateTime) => dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

        public static DateTime Max(this (DateTime a, DateTime b) values)
        {
            return values.a > values.b ? values.a : values.b;
        }

        public static DateTime MaxOrMinValue(this IEnumerable<DateTime> values)
        {
            DateTime max = DateTime.MinValue.KindOfUtc();
     
            foreach (var value in values)
            {
                if (max == DateTime.MinValue)
                    max = value;
                else if (max < value)
                    max = value;
            }

            return max;
        }
    }
}
