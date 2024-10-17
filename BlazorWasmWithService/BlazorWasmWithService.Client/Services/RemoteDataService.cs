using System.Net.Http.Json;
using BlazorWasmWithService.Shared.Data;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmWithService.Client.Services
{
	public class RemoteDataService
	{
        private HttpClient HttpClient { get; set; }

        private NavigationManager NavigationManager { get; set; }
    
        public async Task<WeatherForecast[]> GetData()
        {
            WeatherForecast[]? result = await HttpClient.GetFromJsonAsync<WeatherForecast[]>($"{NavigationManager.BaseUri}api/weather/getdata");

            return result ?? new WeatherForecast[0];
        }

        public RemoteDataService(HttpClient httpClient, NavigationManager navigationManager)
        {
            HttpClient = httpClient;

            NavigationManager = navigationManager;
        }
	}
}
