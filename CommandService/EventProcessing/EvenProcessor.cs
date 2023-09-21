using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing
{
	public class EvenProcessor : IEventProcessor
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IMapper _mapper;

		public EvenProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
		{
			_scopeFactory = scopeFactory;
			_mapper = mapper;
		}
		public void ProcessEvent(string message)
		{
			var evenType = DetermineEvent(message);
			switch (evenType)
			{
				case EventType.PlatformPublished:
					AddPlatform(message); Console.WriteLine("Platform Added");
					break;
				default:
					break;
			}
		}

		private void AddPlatform(string platformPublishedMessage)
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
				var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

				try
				{
					var plat = _mapper.Map<Platform>(platformPublishedDto);
					if (!repo.ExternalPlatformExists(plat.ExternalId))
					{
						repo.CreatePlatform(plat);
						repo.SaveChanges();
					}
					else
					{
                        Console.WriteLine("--> Platform already exists...");
                    }
				}
				catch (Exception ex)
				{
					Console.WriteLine($"--> Could not add Platform to Db {ex.Message}");
				}
			}
		}

		private EventType DetermineEvent(string notificationMessage)
		{
			Console.WriteLine("--> Determining Event");

			var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

			switch (eventType.Event)
			{
				case "Platform_Published":
					Console.WriteLine("--> Platform publish event detected");
					return EventType.PlatformPublished;
				default: Console.WriteLine("--> Could not determine event"); return EventType.Undetermined;
			}
		}
	}

	enum EventType
	{
		PlatformPublished,
		Undetermined
	}
}
