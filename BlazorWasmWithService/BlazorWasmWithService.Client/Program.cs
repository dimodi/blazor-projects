using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using BlazorWasmWithService.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddScoped<LocalDataService>();
builder.Services.AddScoped<RemoteDataService>();

await builder.Build().RunAsync();
