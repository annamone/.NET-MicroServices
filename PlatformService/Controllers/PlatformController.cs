﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlatformController : ControllerBase
	{
		private readonly IPlatformRepo _repostory;
		private readonly IMapper _mapper;
		private readonly ICommandDataClient _commandDataClient;
		private readonly IMessageBusClient _messageBusClient;

		public PlatformController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient,
			IMessageBusClient messageBusClient)
		{
			_repostory = repository;
			_mapper = mapper;
			_commandDataClient = commandDataClient;
			_messageBusClient = messageBusClient;
		}

		[HttpGet]
		public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
		{
			Console.WriteLine("Getting Platforms...");

			var platformItems = _repostory.GetAllPlatforms();

			return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
		}

		[HttpGet("{id}", Name = "GetPlatformById")]
		public ActionResult<PlatformReadDto> GetPlatformById(int id)
		{
			var platformItem = _repostory.GetPlatformById(id);
			if (platformItem != null)
			{
				return Ok(_mapper.Map<PlatformReadDto>(platformItem));
			}

			return NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
		{
			var platformModel = _mapper.Map<Platform>(platformCreateDto);
			_repostory.CreatePlatform(platformModel);
			_repostory.SaveChanges();

			var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

			//Send Sync message
			try
			{
				await _commandDataClient.SendPlatformToCommand(platformReadDto);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
			}

			//Send Async message
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
