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
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			_logger.LogInformation("Migration Worker running at: {time}", DateTimeOffset.UtcNow);
			using var scope = _scopeFactory.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

			await context.Database.MigrateAsync();
			_logger.LogInformation("Migration applied");

			var migrationManager = new MigrationManager();

		}
		else
		{
			_logger.LogInformation("Non Development environment, skipping migration");
		}

		_hostApplication.StopApplication();
	}
}
