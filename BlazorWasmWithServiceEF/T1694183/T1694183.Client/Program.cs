using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using T1694183.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddScoped<ProductServiceClient>();

await builder.Build().RunAsync();
