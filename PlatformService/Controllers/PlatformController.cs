using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformController(IPlatformRepo platformRepo, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }

        // GET: api/platforms
        [HttpGet(Name = "GetPlatforms")]
        public async Task<ActionResult<IEnumerable<Platform>>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms....");
            var platformList = await _platformRepo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformList));
        }

        // GET: api/platforms/{id}
        [HttpGet("{id}", Name = "GetPlatformById")]
        public async Task<ActionResult<Platform>> GetPlatformById(int id)
        {
            Console.WriteLine("--> Getting Platforms By Id....");
            if (id <= 0)
                return BadRequest("Invalid ID. Must be greater than zero.");

            var platform = await _platformRepo.GetPlatformByID(id);
            if (platform == null)
                return NotFound();
            return Ok(_mapper.Map<Platform>(platform));
        }

        // POST: api/platforms
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            if (platformCreateDto == null)
                return BadRequest("Platform data is required.");

            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            try
            {
                _platformRepo.CreatePlatform(platformModel);
                await _platformRepo.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Database error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save platform.");
            }
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Send async message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
