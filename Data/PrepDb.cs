using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using PlatformService.Models;

namespace PlatformService.Data
{

  // db context to generate migration in db
  // used for testing
  public static class PrepDb
  {
    // function to prepare data on application startup
    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
      }
    }

    private static void SeedData(AppDbContext context, bool isProduction)
    {
      if (isProduction)
      {
        Console.WriteLine("Applying Migrations");
        try
        {
          context.Database.Migrate();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Failed To Apply Migrations: {ex.Message}");
        }
      }


      if (!context.Platforms.Any())
      {
        Console.WriteLine("Seeding Data");

        context.Platforms.AddRange(
            // instantiate objects with platform class
            // populate data with used services for test
            new Platform()
            {
              Name = "Dot Net",
              Publisher = "Microsoft",
              Cost = "Free",
            },
            new Platform()
            {
              Name = "SQL Server Express",
              Publisher = "Microsoft",
              Cost = "Free",
            },
            new Platform()
            {
              Name = "Kubernetes",
              Publisher = "Cloud Native Computing Foundation",
              Cost = "Free",
            }
        );

        context.SaveChanges();
      }
      else
      {
        Console.WriteLine("Data is present");
      }
    }
  }
}
