using Newtonsoft.Json;
using System.Data;
using System.Net;
namespace HardWeatherApp.Entities
{
    public static class WeatherApi
    {
        private static readonly HttpClient Client = new();

        public static async Task<WeatherApiResult?> GetWeatherAsync(string zipCode)
        {
            try
            {
                Logger.Log(LogType.INFO, $"(WeatherApi.GetWeatherAsync) Calling API with zip: {zipCode}");

                using var response = await Client.GetAsync($"https://api.openweathermap.org/data/2.5/forecast?zip={zipCode},US&units=imperial&appid={Secret.ApiKey}");

                int status = (int)response.StatusCode;

                if (status != 200)
                {
                    if (status == 404)
                    {
                        Logger.Log(LogType.WARNING, "(WeatherApi.GetWeatherAsync) API could not find city");
                        return new WeatherApiResult()
                        {
                            cod = "404"
                        };
                    }

                    Logger.Log(LogType.ERROR, $"(WeatherApi.GetWeatherAsync) API returned bad status code: {status}");
                    return null;
                }

                var result = JsonConvert.DeserializeObject<WeatherApiResult>(await response.Content.ReadAsStringAsync());

                if (result == null)
                {
                    Logger.Log(LogType.ERROR, "(WeatherApi.GetWeatherAsync) Deserialized result was null");
                    return null;
                }

                foreach (var item in result.list)
                {
                    if (!Valid.Weather(item.weather[0].description))
                    {
                        Logger.Log(LogType.ERROR, "API returned invalid data");
                        return null;
                    }
                }

                return result;
            }
            catch
            {
                Logger.Log(LogType.ERROR, $"(WeatherApi.GetWeatherAsync) Fatal error occurred");
                return null;
            }
        }

    }
}
