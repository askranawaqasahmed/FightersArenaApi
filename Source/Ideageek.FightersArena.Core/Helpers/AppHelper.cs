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
            return _configuration.GetConnectionString("CAFASuiteConnection");
        }
        public string GetPpraUrl()
        {
            return _configuration["AppSettings:PpraUrl"];
        }
        public string GetPpraWithPaginationUrl()
        {
            return _configuration["AppSettings:PpraWithPaginationUrl"];
        }
        public string GetNBPRatesUrl()
        {
            return _configuration["AppSettings:NBPRatesUrl"];
        }
        public string GetPythonPath()
        {
            return _configuration["AppSettings:PythonPath"];
        }
        public string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(false);
            dynamic displayAttribute = null;
            if (attributes.Any())
            {
                displayAttribute = attributes.ElementAt(0);
            }
            return displayAttribute?.Description ?? String.Empty;
        }
        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }
    }
}
