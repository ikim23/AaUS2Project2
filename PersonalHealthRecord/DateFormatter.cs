using System;
using System.Globalization;

namespace PersonalHealthRecord
{
    public class DateFormatter
    {
        public static string Format(DateTime? date) => date != null ? $"{date:dd.MM.yyyy}" : "";

        public static DateTime Parse(string date) => DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        public static DateTime? ParseOrNull(string date)
        {
            if (string.IsNullOrEmpty(date)) return null;
            return DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }
    }
}
