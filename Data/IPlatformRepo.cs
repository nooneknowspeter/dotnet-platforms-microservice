using PlatformService.Models;

namespace PlatformService.Data
{
  // interface for facade design for interact with PlatformRepo
  public interface IPlatformRepo
  {
    // save changes when changing DbContext
    bool SaveChanges();

    // enumerable to for users to retrieve all platforms, 
    // and other information on platforms; facade interaction
    IEnumerable<Platform> GetAllPlatforms();
    Platform GetPlatformById(int id);
    void CreatePlatform(Platform plat);
  }
}
