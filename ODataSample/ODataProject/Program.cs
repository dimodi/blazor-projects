using ClassLibrary;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:7777");
        });
});

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<SampleModel>("SampleModels");

builder.Services.AddControllers().AddOData(
    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
        "odata",
        modelBuilder.GetEdmModel()));   

var app = builder.Build();

app.UseRouting();

app.UseCors();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
