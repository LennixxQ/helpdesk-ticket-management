using HelpDesk.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Infrastructure.BackgroundServices
{
    public class BusinessHoursService : IBusinessHoursService
    {
        private readonly ILogger<BusinessHoursService> _logger;

        // Business hours — IST is UTC+5:30
        private static readonly TimeSpan BusinessStart = new(11, 0, 0);
        private static readonly TimeSpan BusinessEnd = new(20, 30, 0);
        private static readonly TimeZoneInfo IstZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        private static readonly HashSet<DateOnly> IndianPublicHolidays = new()
    {
        // 2026
        new DateOnly(2026, 1, 14),   // Makar Sankranti
        new DateOnly(2026, 1, 26),   // Republic Day
        new DateOnly(2026, 3, 3),    // Holi
        new DateOnly(2026, 3, 20),   // Id-ul-Fitr
        new DateOnly(2026, 3, 30),   // Ram Navami
        new DateOnly(2026, 4, 3),    // Good Friday
        new DateOnly(2026, 4, 14),   // Dr. Ambedkar Jayanti
        new DateOnly(2026, 5, 1),    // Buddha Purnima
        new DateOnly(2026, 8, 15),   // Independence Day
        new DateOnly(2026, 10, 2),   // Gandhi Jayanti
        new DateOnly(2026, 10, 8),   // Dussehra
        new DateOnly(2026, 10, 28),  // Diwali
        new DateOnly(2026, 12, 25),  // Christmas
    };

        public BusinessHoursService(ILogger<BusinessHoursService> logger) => _logger = logger;

        public bool IsWorkingDay(DateOnly date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return false;

            if (IndianPublicHolidays.Contains(date))
                return false;

            return true;
        }

        public bool IsBusinessHour(DateTime utcDateTime)
        {
            var ist = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, IstZone);
            var date = DateOnly.FromDateTime(ist);
            var timeOfDay = ist.TimeOfDay;

            if (!IsWorkingDay(date)) return false;
            return timeOfDay >= BusinessStart && timeOfDay < BusinessEnd;
        }

        public DateTime GetNextBusinessStart(DateTime utcDateTime)
        {
            var ist = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, IstZone);
            var date = DateOnly.FromDateTime(ist);

            // If current time is before business hours today and today is a working day
            if (IsWorkingDay(date) && ist.TimeOfDay < BusinessStart)
            {
                var startToday = ist.Date + BusinessStart;
                return TimeZoneInfo.ConvertTimeToUtc(startToday, IstZone);
            }

            // Otherwise move to next working day
            date = date.AddDays(1);
            while (!IsWorkingDay(date))
                date = date.AddDays(1);

            var nextStart = date.ToDateTime(TimeOnly.FromTimeSpan(BusinessStart));
            return TimeZoneInfo.ConvertTimeToUtc(nextStart, IstZone);
        }

        public DateTime AddBusinessMinutes(DateTime utcStart, int minutes)
        {
            if (minutes <= 0) return utcStart;

            var current = utcStart;
            var remainingMins = minutes;

            // If current time is outside business hours, snap to next business start
            if (!IsBusinessHour(current))
                current = GetNextBusinessStart(current);

            while (remainingMins > 0)
            {
                var ist = TimeZoneInfo.ConvertTimeFromUtc(current, IstZone);
                var endOfDay = ist.Date + BusinessEnd;
                var endOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(endOfDay, IstZone);

                var minutesLeftToday = (int)(endOfDayUtc - current).TotalMinutes;

                if (remainingMins <= minutesLeftToday)
                {
                    current = current.AddMinutes(remainingMins);
                    remainingMins = 0;
                }
                else
                {
                    remainingMins -= minutesLeftToday;
                    current = GetNextBusinessStart(endOfDayUtc);
                }
            }

            return current;
        }
    }
}