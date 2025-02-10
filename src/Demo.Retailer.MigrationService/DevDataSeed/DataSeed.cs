namespace Demo.Retailer.MigrationService
{
	public static class DataSeed
	{
		public static async Task<IHost> SeedDatabaseIfEmptyAsync(this IHost host, int count = 1)
		{

			await using (var scope = host.Services.CreateAsyncScope())
			{
				using (var productsDbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>())
				{
					if (productsDbContext.Products.Any())
					{
						return host;
					}

					var products = new ProductFaker().GetFakeProducts(count);

					productsDbContext.Products.AddRange(products);
					await productsDbContext.SaveChangesAsync();
				}
			}
			return host;
		}

	}
}
