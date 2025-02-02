using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;

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
    }
}
