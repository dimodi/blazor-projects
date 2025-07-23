using Microsoft.EntityFrameworkCore;
using System.Reflection;
using T1694183.Client.Services;
using T1694183.Components;
using T1694183.Data;
using T1694183.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContextFactory<DbContextEF>(contextOptions =>
{
    contextOptions.UseSqlite($"Filename=T1694183.db", sqliteOptions =>
    {
        sqliteOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
    });
});

builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddScoped<ProductServiceClient>();

builder.Services.AddControllers();

builder.Services.AddHttpClient();

builder.Services.AddTelerikBlazor();

var app = builder.Build();

using IServiceScope serviceScope = app.Services.CreateScope();
IDbContextFactory<DbContextEF> dbFactory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<DbContextEF>>();
using DbContextEF dbContext = await dbFactory.CreateDbContextAsync();
await dbContext.Database.EnsureCreatedAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(T1694183.Client._Imports).Assembly);

app.Run();