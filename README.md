# Weather Photo Generator

A simple C# Azure Function that generates a weather photo based on the current weather conditions using Queues and Blob
Storage.

Images are fetched from the [Unsplash API](https://unsplash.com/developers) and weather data is fetched
from [Buienradar](https://www.buienradar.nl).

This project was created as an assignment for the course Cloud Computing at the InHolland University of Applied
Sciences.

## Requirements

- .NET 8
- Azure Functions Core Tools
- Azurite (for local development)
- .NET IDE (Rider, Visual Studio, Visual Studio Code)
- Microsoft Azure account (for deployment)

## How to run locally

1. Fork this repository
2. Grab yourself azurite: `npm install -g azurite`
3. Start azurite with runAzurite.sh
4. Setup `local.settings.json`. Provide the following keys:
    1. `AzureWebJobsStorage` - connection string to the azurite storage account
    2. `BuienradarApiUrl` - the URL to the Buienradar API (base URL)
    3. `UnsplashApiUrl` - the URL to the Unsplash API (base URL)
    4. `UnsplashAccessKey` - the access key to the Unsplash API
    5. `WeatherPhotoContainer` - the name of the container in the storage account where the weather photos are stored
4. Open the project in IDE (in this case, Rider)
5. Start the function app
6. ???
7. Profit!

## Requests

See `api.http` for example requests.

## Deployment

Simply run `./deploy.ps1`. You may want to change the resource group and function app name in the script.

## License

This project is licensed under the PLv1.0 license. See `LICENSE.md` for more information.

```