using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
			builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
			builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
			builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
			builder.Services.AddGrpc();
			builder.Services.AddControllers();
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			app.MapGrpcService<GrpcPlatformService>();

			app.MapGet("/Protos/platforms.proto", async context =>
			{
				await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
			});

			PrepDb.PrepPopulation(app);

			app.Run();
		}
	}
}