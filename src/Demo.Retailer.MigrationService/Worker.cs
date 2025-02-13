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
		_logger.LogInformation("Migration Worker running at: {time}", DateTimeOffset.Now);
		using var scope = _scopeFactory.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			await context.Database.MigrateAsync();
			_logger.LogInformation("Migration applied");
		}

		_hostApplication.StopApplication();
	}
}
