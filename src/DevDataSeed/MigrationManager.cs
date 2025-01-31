using API.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace API.DevDataSeed
{
	public static class MigrationManager
	{
		public static async Task<IHost> MigrateDatabaseAsync(this IHost host)
		{
			await using (var scope = host.Services.CreateAsyncScope())
			{
				using (var productsDbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>())
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
