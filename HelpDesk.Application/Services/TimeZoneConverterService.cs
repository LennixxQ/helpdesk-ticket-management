using HelpDesk.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Application.Services
{
    public class TimeZoneConverterService : ITimeZoneConverterService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TimeZoneConverterService> _logger;
        private readonly TimeZoneInfo _timeZone;

        public TimeZoneConverterService(IConfiguration config, ILogger<TimeZoneConverterService> logger)
        {
            _config = config;
            _logger = logger;
            
            var tzString = _config["AppSettings:DefaultTimeZone"] ?? "UTC";
            try
            {
                _timeZone = TimeZoneInfo.FindSystemTimeZoneById(tzString);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not find time zone '{TimeZone}'. Falling back to UTC.", tzString);
                _timeZone = TimeZoneInfo.Utc;
            }
        }

        public DateTime ConvertToLocal(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _timeZone);
        }

        public string GetTimeZoneAbbreviation()
        {
            if (_timeZone.Id == "UTC") return "UTC";
            
            // Basic mapping for common time zones (Windows standard names -> Abbreviations)
            // Ideally, this would use a robust library like TimeZoneNames, but this works for simple setups
            return _timeZone.Id switch
            {
                "India Standard Time" => "IST",
                "Eastern Standard Time" => _timeZone.IsDaylightSavingTime(DateTime.UtcNow) ? "EDT" : "EST",
                "Central Standard Time" => _timeZone.IsDaylightSavingTime(DateTime.UtcNow) ? "CDT" : "CST",
                "Mountain Standard Time" => _timeZone.IsDaylightSavingTime(DateTime.UtcNow) ? "MDT" : "MST",
                "Pacific Standard Time" => _timeZone.IsDaylightSavingTime(DateTime.UtcNow) ? "PDT" : "PST",
                "GMT Standard Time" => _timeZone.IsDaylightSavingTime(DateTime.UtcNow) ? "BST" : "GMT",
                _ => _timeZone.StandardName
            };
        }
    }
}
