using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder appBuilder)
		{
			using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
			}
		}

		private static void SeedData(AppDbContext context)
		{
			if (!context.Platforms.Any())
			{
				Console.WriteLine("--> Seeding Data");
				context.Platforms.AddRange(
					new Platform()
					{
						Name = "Dot Net",
						Publisher = "Microsoft",
						Cost = "Free"
					},
					new Platform()
					{
						Name = "Sql Server Express",
						Publisher = "Microsoft",
						Cost = "Free"
					},
					new Platform()
					{
						Name = "Kubernetes",
						Publisher = "Cloud Native Computing Foundation",
						Cost = "Free"
					}
					);
				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have data");
			}
		}
	}
}
