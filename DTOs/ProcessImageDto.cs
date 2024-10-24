using Athlon.WeatherPhoto.Models;

namespace Athlon.WeatherPhoto.DTOs;

public record ProcessImageDto(string JobId, string WeatherDescription, string StationName, WeatherStation Station);