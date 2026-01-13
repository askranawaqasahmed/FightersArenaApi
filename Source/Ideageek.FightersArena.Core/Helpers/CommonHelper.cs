using System.Text.RegularExpressions;

namespace Ideageek.FightersArena.Core.Helpers
{
    public static class CommonHelper
    {
        public static bool ParseToBool(string value)
        {
            return Convert.ToBoolean(value);
        }
        public static bool ParseToBool(int value)
        {
            return Convert.ToBoolean(value);
        }
        public static int ParseToInt(string value)
        {
            return Convert.ToInt32(value);
        }
        public static Guid ParseToGuid(string value)
        {
            return Guid.Parse(value);
        }
        public static DateTime StringToDateTime(string datetime)
        {
            return DateTime.Parse(datetime);
        }
        public static string DateTimeToString(DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy");
        }
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }
        public static string GenerateCode()
        {
            return String.Format("{0} {1}", "IMC-", Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper());
        }
        public static DateTime GetDateWithTime(DateTime date)
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            DateTime dateTime = date + time;
            return dateTime;
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static IEnumerable<T> ParseEnum<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        public static string GetEnumDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field is null) return string.Empty;

            var displayAttribute = field.GetCustomAttributes(false).FirstOrDefault();
            var description = displayAttribute?.GetType().GetProperty("Description")?.GetValue(displayAttribute) as string;
            return description ?? string.Empty;
        }
        public static bool IsValidEnum<T>(this Enum value)
        {
            return Enum.IsDefined(typeof(T), value);
        }
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        public static string? GetTextFromRegex(string text, string keyword)
        {
            var match = Regex.Match(text, keyword + @"\s+(\d+)");
            if (match.Success)
                return match.Groups[1].Value;
            else
                return null;
        }
        public static string RemoveTrailingNumber(string input)
        {
            // Use regex to remove trailing numbers with optional spaces before them
            return Regex.Replace(input, @"\s*\d+$", "").Trim();
        }
        public static string[] SplitNewLine(string input)
        {
            return input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }
        public static string GenerateJobNumber(string clientCode, int? serialNumber)
        {
            serialNumber = serialNumber ?? 1;
            return string.Format("{0}-{1}/{2}", clientCode, serialNumber, DateTime.Now.Year.ToString("yy"));
        }
        public static string GeneratePayOrderNumber(int payOrderCount)
        {
            return string.Format("{0}/{1}-{2}/{3}", "MAMM", "PO", payOrderCount + 1, DateTime.Now.Year.ToString("yy"));
        }
        public static string GenerateLetterNumber(string bankCode, int serialNumber)
        {
            return string.Format("{0}/{1}-{2}/{3}", "MAMM", bankCode, serialNumber, DateTime.Now.Year.ToString("yy"));
        }
        public static string GenerateInvoiceNumber(string clientCode, int? serialNumber)
        {
            serialNumber = serialNumber ?? 1;
            return string.Format("IN-{0}-{1}/{2}", clientCode, serialNumber, DateTime.Now.Year % 100);
        }
        public static string GetNameCode(string fullName)
        {
            string[] names = fullName.Split(' ');
            if (names.Length == 2)
                return string.Format("{0}{1}", names[0].Substring(0, 1), names[1].Substring(0, 1)).ToUpper();
            else
                return fullName.Substring(0, 2).ToUpper();
        }
    }
}
