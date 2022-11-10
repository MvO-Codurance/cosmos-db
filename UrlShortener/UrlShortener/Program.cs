using Microsoft.AspNetCore.HttpLogging;
using UrlShortener.Modules;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Ensure we take the environment name from the environment variables.
    // This is for the integration tests so we can pick up the "appsettings.Testing.json" file. 
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
});

// Add services to the container.
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath | HttpLoggingFields.RequestQuery | HttpLoggingFields.RequestBody |
                            HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.ResponseBody;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterModules(builder.Configuration);

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

// required for UrlShortner.IntegrationTests to run as WebApplicationFactory needs an entry point
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program { }
