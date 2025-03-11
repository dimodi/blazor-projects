using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using DockerBlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTelerikBlazor();

// SignalR max message size for Editor, FileManager, FileSelect, PDF Viewer, Signature
builder.Services.Configure<HubOptions>(options =>
{
    //options.MaximumReceiveMessageSize = 32 * 1024; // 32 KB, or use null for no limit
});

// .NET Core max form body length for Upload
builder.Services.Configure<FormOptions>(options =>
{
    //options.MultipartBodyLengthLimit = 128 * 1048576; // 128 MB
});

// Kestrel max request body size for Upload
builder.Services.Configure<KestrelServerOptions>(options =>
{
    //options.Limits.MaxRequestBodySize = 28 * 1048576; // 28 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();