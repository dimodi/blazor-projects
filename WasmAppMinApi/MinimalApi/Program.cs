using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using WasmAppMinApi.Shared;

const string ALLOW_CORS_POLICY_NAME = "CORS_POLICY_UPLOAD";
const string ALLOW_CORS_ORIGIN = "https://localhost:7001";
const string ANTIFORGERY_COOKIE_NAME = "XSRF-TOKEN-UPLOAD";
const string UPLOADS_FOLDER = "uploads";

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 128 * 1024 * 1024;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 128 * 1024 * 1024;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: ALLOW_CORS_POLICY_NAME,
                      builder =>
                      {
                          builder.AllowAnyHeader();
                          builder.AllowAnyMethod();
                          builder.AllowCredentials();
                          builder.WithOrigins(ALLOW_CORS_ORIGIN);
                      });
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ANTIFORGERY_COOKIE_NAME;
});

var app = builder.Build();

app.UseAntiforgery();

var antiforgery = app.Services.GetRequiredService<IAntiforgery>();

app.UseCors(ALLOW_CORS_POLICY_NAME);

app.MapGet("/api/upload/token", (HttpContext context, IAntiforgery antiforgery) =>
{
    var tokenSet = antiforgery.GetTokens(context);

    var afData = new AntiForgeryData(
        ANTIFORGERY_COOKIE_NAME,
        tokenSet.CookieToken ?? string.Empty,
        tokenSet.FormFieldName,
        tokenSet.HeaderName ?? string.Empty,
        tokenSet.RequestToken ?? string.Empty
    );

    return TypedResults.Ok(afData);
});

app.MapPost("/api/upload/save", [EnableCors(ALLOW_CORS_POLICY_NAME)] async (IFormFile files) =>
{
    if (files != null)
    {
        try
        {
            var rootPath = app.Environment.ContentRootPath;
            var saveLocation = Path.Combine(rootPath, UPLOADS_FOLDER, files.FileName);
            using (var fileStream = new FileStream(saveLocation, FileMode.Create))
            {
                await files.CopyToAsync(fileStream);
            }

            Results.Ok();
        }
        catch (Exception ex)
        {
            Results.Problem(null, null, 500, $"Upload Failed: {ex.Message}");
        }
    }
});

app.MapPost("/api/upload/remove", [EnableCors(ALLOW_CORS_POLICY_NAME)] ([FromForm] string files) =>
{
    if (files != null)
    {
        try
        {
            var rootPath = app.Environment.ContentRootPath;
            var fileLocation = Path.Combine(rootPath, UPLOADS_FOLDER, files);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }

            Results.Ok();
        }
        catch (Exception ex)
        {
            Results.Problem(null, null, 500, $"Delete Failed: {ex.Message}");
        }
    }
});

#region Minimal API 101

app.MapGet("/", [EnableCors(ALLOW_CORS_POLICY_NAME)] () => "Hello World!");

app.MapGet("/get", () =>
{
    return $"GET Response at {DateTime.Now.ToString("HH:mm:ss.fff")}.";
});

app.MapGet("/get/{arg}", [EnableCors(ALLOW_CORS_POLICY_NAME)] (string arg) =>
{
    return $"GET response for {arg} at {DateTime.Now.ToString("HH:mm:ss.fff")}.";
});

app.MapPost("/post", [EnableCors(ALLOW_CORS_POLICY_NAME)] ([FromForm] string arg) =>
{
    return $"POST response for {arg} at {DateTime.Now.ToString("HH:mm:ss.fff")}";
}).DisableAntiforgery();

#endregion Minimal API 101

app.Run();
