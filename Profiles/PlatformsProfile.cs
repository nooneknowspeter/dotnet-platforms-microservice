using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
  // map DTOs to Models using AutoMapper; adapter design
  public class PlatformsProfile : Profile
  {
    public PlatformsProfile()
    {
      // maps source data to target data
      // maps platform target to read DTO and provides data to end user
      CreateMap<Platform, PlatformReadDto>();
      CreateMap<PlatformCreateDto, Platform>();
    }
  }
}
