namespace CareSchedule.Shared.Time
{
    public static class TimeZoneHelper
    {
        /// <summary>
        /// Returns the IST time zone. Works on Windows ("India Standard Time") and Linux ("Asia/Kolkata").
        /// </summary>
        public static TimeZoneInfo GetIstTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"); } // Windows
            catch { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata"); }     // Linux
        }

        /// <summary>
        /// Convert UTC DateTime (as stored in DB) to IST DateTimeOffset (+05:30).
        /// </summary>
        public static DateTimeOffset ToIst(DateTime utc)
        {
            var utcDto = new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc), TimeSpan.Zero);
            return TimeZoneInfo.ConvertTime(utcDto, GetIstTimeZone());
        }

        /// <summary>
        /// Convert any DateTimeOffset (with any offset) to UTC DateTime for storage.
        /// </summary>
        public static DateTime ToUtc(DateTimeOffset any) => any.UtcDateTime;
    }
}