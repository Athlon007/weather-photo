using Athlon.WeatherPhoto.Interfaces;
using Newtonsoft.Json;

namespace Athlon.WeatherPhoto.Services;

public class ImageService(HttpClient httpClient) : IService
{
    private readonly string _apiUrl = Environment.GetEnvironmentVariable("UnsplashApiUrl") ??
                                      throw new ArgumentNullException("UnsplashApiUrl is not set.");
    private readonly string _access = Environment.GetEnvironmentVariable("UnsplashAccessKey") ??
                                      throw new ArgumentNullException("UnsplashAccessKey is not set.");

    private async Task<dynamic> FetchImagesByQuery(string query)
    {
        // Add the required headers
        var parameters = new Dictionary<string, string>
        {
            {"query", query},
            {"client_id", _access}
        };
        string paramsString = string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}"));
        var response = await httpClient.GetAsync($"{_apiUrl}/search/photos?{paramsString}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get image data.");
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(content) ?? throw new ArgumentNullException("Failed to deserialize image data.");
    }

    public async Task<Stream> GetRandomImage(string query)
    {
        dynamic data = await FetchImagesByQuery(query);

        // Pick random image
        Random random = new Random(DateTime.Now.Millisecond);
        int index = random.Next(0, data.results.Count);
        string url = data.results[index].urls.regular.ToString().Split("?")[0];

        // Download the image file.
        byte[] imageBytes = await httpClient.GetByteArrayAsync(url);

        return new MemoryStream(imageBytes);
    }
}