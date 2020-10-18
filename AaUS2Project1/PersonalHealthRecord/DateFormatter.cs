using System;
using System.Globalization;

namespace PersonalHealthRecord
{
    public class DateFormatter
    {
        public static string ToString(DateTime? date)
        {
            if (date == null) return null;
            return $"{date:dd.MM.yyyy hh:mm:ss.fff}";
        }

        public static DateTime Parse(string date) => DateTime.ParseExact(date, "dd.MM.yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture);

        public static DateTime? ParseOrNull(string date)
        {
            if (string.IsNullOrEmpty(date)) return null;
            return DateTime.ParseExact(date, "dd.MM.yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture);
        }
    }
}
