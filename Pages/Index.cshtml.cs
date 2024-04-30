using HardWeatherApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.ObjectPool;
using System.Reflection.Metadata.Ecma335;

namespace HardWeatherApp.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {

        }

        public void OnGet()
        {

        }

        public async Task<JsonResult> OnGetSubmit()
        {
            try
            {
                var zipCode = (string?)Request.Query["ZipCode"];

                if (!Valid.ZipCode(zipCode))
                    return new JsonResult("invalid");

                var apiResult = await WeatherApi.GetWeatherAsync(zipCode!);

                if (apiResult is null)
                    return new JsonResult("error");

                if (apiResult.cod == "404")
                    return new JsonResult("404");

                var threeDayForecast = new List<List>();

                DateTime? currentDate = null;

                var i = 0;

                foreach (var item in apiResult.list)
                {
                    if (i == 3)
                        break;

                    var dateTime = GetDate(item.dt);

                    if (currentDate != null && dateTime.Day == currentDate.Value.Day)
                        continue;

                    currentDate = dateTime;

                    threeDayForecast.Add(item);

                    i++;
                }

                var forecast = new CityForecast()
                {
                    City = apiResult.city.name,
                    Forecast = threeDayForecast
                };

                var aves = GetAves(apiResult);

                for (i = 0; i < 3; i++)
                {
                    forecast.Forecast[i].main.temp = aves[i][0];
                    forecast.Forecast[i].wind.speed = aves[i][1];
                }

                return new JsonResult(forecast);
            }
            catch
            {
                Logger.Log(LogType.ERROR, "A unhandled exception occured");
                return new JsonResult("error");
            }
        }

        private static DateTime GetDate(double dt)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dt).ToLocalTime();
        }

        private static int[][] GetAves(WeatherApiResult data)
        {
            var currentDay = GetDate(data.list[0].dt);
            double tempTotal = 0;
            double windTotal = 0;
            var count = 0;
            var repeat = 0;

            var result = new List<int[]>();

            foreach (var i in data.list)
            {
                if (repeat == 3)
                    break;

                var date = GetDate(i.dt);

                if (date.Day == currentDay.Day)
                {
                    tempTotal += i.main.temp;
                    windTotal += i.wind.speed;
                    count++;
                }
                else
                {
                    result.Add(
                    [
                        (int)Math.Round(tempTotal / count),
                        (int)Math.Round(windTotal / count)
                    ]);
                    currentDay = date;
                    tempTotal = i.main.temp;
                    windTotal = i.wind.speed;
                    count = 1;
                    repeat++;
                }
            }
            return [.. result];
        }

    }
}
