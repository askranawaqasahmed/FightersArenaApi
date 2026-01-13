using Microsoft.Extensions.Configuration;

namespace Ideageek.FightersArena.Core.Helpers
{
    public interface IAppHelper
    {
        string GetConnectionString();
        string GetPpraUrl();
        string GetPpraWithPaginationUrl();
        string GetNBPRatesUrl();
        string GetPythonPath();
        string GetEnumDescription(Enum value);
        IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate);
    }
    public class AppHelper : IAppHelper
    {
        private readonly IConfiguration _configuration;
        public AppHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("CAFASuiteConnection") ?? string.Empty;
        }
        public string GetPpraUrl()
        {
            return _configuration["AppSettings:PpraUrl"] ?? string.Empty;
        }
        public string GetPpraWithPaginationUrl()
        {
            return _configuration["AppSettings:PpraWithPaginationUrl"] ?? string.Empty;
        }
        public string GetNBPRatesUrl()
        {
            return _configuration["AppSettings:NBPRatesUrl"] ?? string.Empty;
        }
        public string GetPythonPath()
        {
            return _configuration["AppSettings:PythonPath"] ?? string.Empty;
        }
        public string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field is null) return string.Empty;

            var displayAttribute = field.GetCustomAttributes(false).FirstOrDefault();
            var description = displayAttribute?.GetType().GetProperty("Description")?.GetValue(displayAttribute) as string;
            return description ?? string.Empty;
        }
        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1))
            {
                yield return date;
            }
        }
    }
}
