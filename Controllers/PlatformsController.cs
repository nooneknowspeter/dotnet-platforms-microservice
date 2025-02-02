using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [ApiController]
    // route to api endpoint, "api/[controller]" -> "api/platforms"
    [Route("/api/[controller]")]
    // using controller base as a simple controller for api
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // returns enmuration of PlatformReadDTO when the api endpoint
        // gets a GET request to the api platforms route
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("Getting Platforms From Endpoint");

            // uses dependency injection, returns platforms information
            // using enumerable specified in interface
            var platformItem = _repository.GetAllPlatforms();

            // mapping platform module data to DTO using method from interface enum
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }

        // added new route with parameter of id
        // gets platform by id from database
        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _repository.GetPlatformById(id);

            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            return NotFound();
        }

        // creates platform through DTO and adds to model
        [HttpPost]
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            // linking adapter to pass in DTO to model using AutoMapper; mapper dependency
            var platformModel = _mapper.Map<Platform>(platformCreateDto);

            _repository.CreatePlatform(platformModel);

            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // returns URI / URL of newly created platform in model / database
            return CreatedAtRoute(
                // name of action
                nameof(GetPlatformById),
                new { Id = platformReadDto.Id },
                platformReadDto
            );
        }
    }
}
