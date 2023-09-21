using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace CommandService.Controllers
{
	[Route("api/c/platforms/{platformId}/[controller]")]
	[ApiController]
	public class CommandController : ControllerBase
	{
		private readonly ICommandRepo _repository;
		private readonly IMapper _mapper;

		public CommandController(ICommandRepo repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		[HttpGet]
		public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
		{
			Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

			if (!_repository.PlarformExists(platformId))
			{
				return NotFound();
			}

			var commands = _repository.GetCommandsForPlatform(platformId);

			return Ok(_mapper.Map<CommandReadDto>(commands));
		}


		[HttpGet("{commandId}", Name = "GetCommandFormPlaform")]
		public ActionResult<IEnumerable<CommandReadDto>> GetCommandFormPlaform(int platformId, int commandId)
		{
			Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId} / {commandId}");

			if (!_repository.PlarformExists(platformId))
			{
				return NotFound();
			}

			var command = _repository.GetCommand(platformId, commandId);

			if (command == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<CommandReadDto>(command));
		}

		[HttpPost]
		public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
		{
			Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

			if (!_repository.PlarformExists(platformId))
			{
				return NotFound();
			}

			var command = _mapper.Map<Command>(commandDto);

			_repository.CreateCommand(platformId, command);
			_repository.SaveChanges();

			var commandReadDto = _mapper.Map<CommandReadDto>(command);

			return CreatedAtRoute(nameof(GetCommandFormPlaform), new { platformId = platformId, commandId = commandReadDto.Id });
		}
	}
}
