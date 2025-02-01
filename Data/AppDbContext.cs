using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{

  public class AppDbContext : DbContext
  {
    // shell constructor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Platform> Platforms { get; set; }

  }

}
