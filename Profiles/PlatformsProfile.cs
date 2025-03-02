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
      // maps Platform object to gRPC Object
      CreateMap<Platform, GrpcPlatformModel>()
        .ForMember(destination => destination.PlatformId, option => option.MapFrom(source => source.Id))
        .ForMember(destination => destination.Name, option => option.MapFrom(source => source.Name))
        .ForMember(destination => destination.Publisher, option => option.MapFrom(source => source.Publisher));
    }
  }
}
