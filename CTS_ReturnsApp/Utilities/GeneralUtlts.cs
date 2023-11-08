using CTS_ReturnsApp.DataAccess;
using CTS_ReturnsApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CTS_ReturnsApp.Utilities
{
    public class GeneralUtlts : Controller
    {
        TimeZoneInfo PortlandZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        TimeZoneInfo SaltilloZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
        TimeZoneInfo UTCZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        public DateTime ConvertPortlandTimeToSaltilloByDate(DateTime DateToConvert)
        {
            DateTime Ut = TimeZoneInfo.ConvertTimeToUtc(DateToConvert, PortlandZone);
            return TimeZoneInfo.ConvertTime(Ut, SaltilloZone);
        }

        public DateTime ConvertSaltilloTimeToPorlantByDate(DateTime DateToConvert)
        {
            DateTime Ut = TimeZoneInfo.ConvertTimeToUtc(DateToConvert, SaltilloZone);
            return TimeZoneInfo.ConvertTime(Ut, PortlandZone);
        }
    }
}
