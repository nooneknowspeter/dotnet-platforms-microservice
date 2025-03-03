using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataService.Http;
using PlatformService.SyncDataServices.Grpc;

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

// configure asynchronous messaging
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

// configure gRPC
builder.Services.AddGrpc();

const string serviceName = "PlatformService";

// enable CORS
builder.Services.AddCors();

//configure AppDbContext
if (isProductionSource)
{
  // database setup
  // 
  //-------------------------------------------------------------------------------------
  Console.WriteLine("--> Using Database");

  builder.Services.AddDbContext<AppDbContext>(option =>
      option.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection"))
  );

  // opentelemetry setup
  // uses HTTP, protobuf --> http://lgtm-clusterip-service:4318
  // 
  //-------------------------------------------------------------------------------------
  Console.WriteLine("S --> Configuring OpenTelemetry");

  var openTelemetryURI = builder.Configuration.GetConnectionString("OpenTelemetryHTTP");
  Console.WriteLine($"OpenTelemetry Endpoint --> {openTelemetryURI}");

  builder.Services.AddOpenTelemetry()
      .WithTracing(tracing => tracing
          // The rest of your setup code goes here
          .AddOtlpExporter(options =>
          {
            options.Endpoint = new Uri(openTelemetryURI + "/v1/traces");
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
          }))
      .WithMetrics(metrics => metrics
          // The rest of your setup code goes here
          .AddOtlpExporter(options =>
          {
            options.Endpoint = new Uri(openTelemetryURI + "/v1/metrics");
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
          }));

  builder.Logging.AddOpenTelemetry(logging =>
  {
    // The rest of your setup code goes here
    logging.AddOtlpExporter(options =>
    {
      options.Endpoint = new Uri(openTelemetryURI + "/v1/logs");
      options.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
  });

  // set production to true
  // builds application for production
  // 
  //-------------------------------------------------------------------------------------
  isProduction = true;
}
else
{
  Console.WriteLine("--> Using RAM");

  builder.Services.AddDbContext<AppDbContext>(
      // database setup
      // using inMemory / caching during development for quick interaction and testing
      // connect db for production to save state
      // 
      //-------------------------------------------------------------------------------------
      option =>
      option.UseInMemoryDatabase("InMem")
  );

  // opentelemetry setup
  // uses HTTP, protobuf --> http://localhost:4318
  // 
  //-------------------------------------------------------------------------------------
  Console.WriteLine("S --> Configuring OpenTelemetry");

  var openTelemetryURI = builder.Configuration.GetConnectionString("OpenTelemetryHTTP");
  Console.WriteLine($"OpenTelemetry Endpoint --> {openTelemetryURI}");

  builder.Services.AddOpenTelemetry()
      .WithTracing(tracing => tracing
          // the rest of your setup code goes here
          .AddOtlpExporter(options =>
          {
            options.Endpoint = new Uri(openTelemetryURI);
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
          }))
      .WithMetrics(metrics => metrics
          // the rest of your setup code goes here
          .AddOtlpExporter(options =>
          {
            options.Endpoint = new Uri(openTelemetryURI);
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
          })
          );

  builder.Logging.AddOpenTelemetry(logging =>
  {
    // The rest of your setup code goes here
    logging.AddOtlpExporter(options =>
    {
      options.Endpoint = new Uri(openTelemetryURI);
      options.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
  });


  // set production to false
  // builds application for development
  // 
  //-------------------------------------------------------------------------------------
  isProduction = false;
}


var app = builder.Build();

// setup PrepDb class to apply migration during production
PrepDb.PrepPopulation(app, isProduction);

// setup CORS
if (isProduction)
{
  app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithOrigins("http://frontend-clusterip-service:3000"));
}
else
{
  app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithOrigins("http://localhost:5173"));
}

app.UseHttpsRedirection();

// map controllers to endpoints
app.MapControllers();

// map gRPC endpoints
app.MapGrpcService<GrpcPlatformService>();

// endpoint to serve gRPC contract
app.MapGet("/protos/platforms.proto", async context =>
{
  await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

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
