using Microsoft.EntityFrameworkCore;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at
// https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure AppDbContext 
builder.Services.AddDbContext<AppDbContext>(
    // using inMemory / caching during development for quick interaction and testing
    // connect db for production to save state
    option => option.UseInMemoryDatabase("InMem"));

// map concrete type from interface to PlatformRepo class; dependency injection
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

PrepDb.PrepPopulation(app);

app.UseHttpsRedirection();

var summaries = new[] {
  "Freezing", "Bracing", "Chilly", "Cool",       "Mild",
  "Warm",     "Balmy",   "Hot",    "Sweltering", "Scorching",
};

app.MapGet("/weatherforecast", () =>
{
  var forecast =
      Enumerable.Range(1, 5)
          .Select(index => new WeatherForecast(
                      DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                      Random.Shared.Next(-20, 55),
                      summaries[Random.Shared.Next(summaries.Length)]))
          .ToArray();
  return forecast;
}).WithName("GetWeatherForecast").WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
