using System.Text.RegularExpressions;

namespace HardWeatherApp.Entities
{
    public static class Valid
    {
        public static bool ZipCode(string? zipCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
                return false;

            return Regex.IsMatch(zipCode, @"^[0-9]{5}$");
        }

        public static bool Log(string? log)
        {
            if (string.IsNullOrWhiteSpace(log))
                return false;

            return Regex.IsMatch(log, @"^[A-Za-z0-9 :().]{3,200}$");
        }

        public static bool Weather(string? weather)
        {
            if (string.IsNullOrWhiteSpace(weather))
                return false;

            return Regex.IsMatch(weather, @"^[A-Za-z ]{2,50}$");
        }
    }
}
