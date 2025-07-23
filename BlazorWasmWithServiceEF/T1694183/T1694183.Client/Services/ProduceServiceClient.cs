using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using T1694183.Shared.Data;
using Telerik.DataSource;

namespace T1694183.Client.Services
{
	public class ProductServiceClient
	{
        private HttpClient HttpClient { get; set; }

        private NavigationManager NavigationManager { get; set; }
    
        public async Task<DataEnvelope<Product>> Read(DataSourceRequest request)
        {
            HttpResponseMessage response = await HttpClient.PostAsJsonAsync($"{NavigationManager.BaseUri}api/product/read", request);

            DataEnvelope<Product>? result = default;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = await response.Content.ReadFromJsonAsync<DataEnvelope<Product>>();
            }

            return result ?? new();
        }

        public ProductServiceClient(HttpClient httpClient, NavigationManager navigationManager)
        {
            HttpClient = httpClient;

            NavigationManager = navigationManager;
        }
	}
}