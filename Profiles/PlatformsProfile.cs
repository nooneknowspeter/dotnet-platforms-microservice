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
      CreateMap<Platform, PlatformReadDto>();
      CreateMap<PlatformCreateDto, Platform>();
      CreateMap<PlatformReadDto, PlatformPublishedDto>();
    }
  }
}
