using Athlon.WeatherPhoto.Interfaces;
using Athlon.WeatherPhoto.Models;
using Newtonsoft.Json;

namespace Athlon.WeatherPhoto.Services;

public class WeatherService : IService
{
    private async Task<dynamic> GetWeatherData()
    {
        var apiUrl = Environment.GetEnvironmentVariable("BuienradarApiUrl") ??
                     throw new ArgumentNullException("BuienradarApiUrl is not set.");

        // Make a request to the weather API
        using var client = new HttpClient();
        var response = await client.GetAsync($"{apiUrl}/2.0/feed/json");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get weather data.");
        }

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject(content) ??
               throw new ArgumentNullException("Failed to deserialize weather data.");
    }

    public async Task<WeatherStation> GetWeatherStationData(int stationId)
    {
        var weatherData = await GetWeatherData();

        var stations = weatherData.actual.stationmeasurements;
        dynamic? station = null;
        foreach (var s in stations)
        {
            if (s.stationid != stationId) continue;
            station = s;
            break;
        }

        // Convert station to WeatherStation object
        if (station == null)
        {
            throw new KeyNotFoundException("Failed to get weather data for the specified station.");
        }

        return new WeatherStation
        {
            weatherdescription = station.weatherdescription,
            temperature = station.temperature,
            windspeed = station.windspeed,
            winddirection = station.winddirection,
            airpressure = station.airpressure,
            humidity = station.humidity
        };
    }
}
