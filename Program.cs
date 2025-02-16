using System.Configuration;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);

// environment variables pre-build
var isDevelopmentSource = builder.Environment.IsDevelopment();
var isProductionSource = builder.Environment.IsProduction();

// determine environment
var isProduction = false;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at
// https://aka.ms/aspnetcore/swashbuckle

// added controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// map concrete type from interface to PlatformRepo class; dependency injection
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

// configure AutoMapper for dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

//configure AppDbContext
if (!isProductionSource)
{
    Console.WriteLine("Using Database");

    builder.Services.AddDbContext<AppDbContext>(option =>
        option.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection"))
    );

    isProduction = true;
}
else
{
    Console.WriteLine("Using RAM");

    builder.Services.AddDbContext<AppDbContext>(
        // using inMemory / caching during development for quick interaction and testing
        // connect db for production to save state
        option =>
        option.UseInMemoryDatabase("InMem")
    );

    isProduction = false;
}

var app = builder.Build();

// setup PrepDb class to apply migration during production
PrepDb.PrepPopulation(app, isProduction);

app.UseHttpsRedirection();

// map controllers to endpoints
app.MapControllers();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

// mock endpoint for testing from dotnet template
app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
