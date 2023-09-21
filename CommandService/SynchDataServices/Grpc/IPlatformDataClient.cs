using CommandService.Models;

namespace CommandService.SynchDataServices.Grpc
{
	public interface IPlatformDataClient
	{
		IEnumerable<Platform> ReturnAllPlatforms();
	}
}
