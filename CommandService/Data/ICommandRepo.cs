using CommandService.Models;

namespace CommandService.Data
{
	public interface ICommandRepo
	{
		bool SaveChanges();

		//Platforms
		IEnumerable<Platform> GetAllPlatforms();
		void CreatePlatform(Platform platform);
		bool PlarformExists(int platformId);
		bool ExternalPlatformExists(int externalPlatformId);

		//Commands
		IEnumerable<Command> GetCommandsForPlatform(int platformId);
		Command GetCommand(int platformid, int commandId);
		void CreateCommand(int platformId, Command command);
	}
}
