using API.DataAccess;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace RetailApiTestProject
{
	public static class SetupInMemoryDbContext
	{
		public static ProductsDbContext GetProductsDbContext(string dbName)
		{
			//set up the options to use for this dbcontext
			var options = new DbContextOptionsBuilder<ProductsDbContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				//.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
				.Options;

			var dbContext = new ProductsDbContext(options);

			return dbContext;
		}
		public static void SeedAppDbContext(this ProductsDbContext weatherForecastDbcontext, List<Product> workDays)
		{
			// add companies


			weatherForecastDbcontext.Products.AddRange(workDays);
			weatherForecastDbcontext.SaveChanges();

			//and then to detach everything 
			//foreach ( var entity in weatherForecastDbcontext.ChangeTracker.Entries() ) {
			//    entity.State = EntityState.Detached;
			//}
		}
	}
}
