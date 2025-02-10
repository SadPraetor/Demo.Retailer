using Microsoft.EntityFrameworkCore;

namespace Demo.Retailer.MigrationService
{
	public static class MigrationManager
	{
		public static async Task<IHost> MigrateDatabaseAsync(this IHost host)
		{
			await using (var scope = host.Services.CreateAsyncScope())
			{
				using (var productsDbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>())
				{
					try
					{
						await productsDbContext.Database.MigrateAsync();
					}
					catch (Exception ex)
					{
						//Log errors or do anything you think it's needed
						Console.WriteLine("Issue when loading dev data: " + ex.Message);
						throw;
					}
				}
			}
			return host;
		}
	}
}
