using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ICommandRepo _commandRepo;
        private readonly IMapper _mapper;
        public CommandController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

            // check platform exists
            if (!await _commandRepo.PlatformExists(platformId))
                return NotFound();
            var commands = _commandRepo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            // check platform exists
            if (!await _commandRepo.PlatformExists(platformId))
                return NotFound($"Platform with ID {platformId} was not found.");

            // check command exists
            var command = _commandRepo.GetCommand(platformId, commandId);
            if (command == null)
                return NotFound($"Command with ID {commandId} for platform {platformId} was not found.");

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            // check platform exists
            if (!await _commandRepo.PlatformExists(platformId))
                return NotFound($"Platform with ID {platformId} was not found.");

            // check command data available
            if (commandDto == null)
                return BadRequest("Command data is required.");
            try
            {
                // create command for platform
                var command = _mapper.Map<Command>(commandDto);
                _commandRepo.CreateCommand(platformId, command);
                await _commandRepo.SaveChanges();

                var commandReadDto = _mapper.Map<CommandReadDto>(command);
                return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
