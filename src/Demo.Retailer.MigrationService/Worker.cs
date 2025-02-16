using Demo.Retailer.MigrationService.DevDataSeed;
using Microsoft.EntityFrameworkCore;

namespace Demo.Retailer.MigrationService;

public class Worker : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IHostApplicationLifetime _hostApplication;
	private readonly ILogger<Worker> _logger;

	public Worker(IServiceScopeFactory scopeFactory,
		IHostApplicationLifetime hostApplication,
		ILogger<Worker> logger)
	{
		_scopeFactory = scopeFactory;
		_hostApplication = hostApplication;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Migration Worker running at: {time}", DateTimeOffset.UtcNow);
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			using var scope = _scopeFactory.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
			using (var _ = new ActionTimer((elapsed)=>_logger.LogInformation("Migration done in {elapsed}",elapsed)))
			{
				await context.Database.MigrateAsync();			
			}

			var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeedManager>>();

			var seedManager = new DataSeedManager(logger);

			using (var _ = new ActionTimer((elapsed)=> _logger.LogInformation("Data seed done in {elapsed}", elapsed)))
			{
				await seedManager.RunSeedOperationAsync(context, SeedSize.Medium);
			}

		}
		else
		{
			_logger.LogInformation("Non Development environment, skipping migration");
		}

		_hostApplication.StopApplication();
	}
}
