namespace Demo.Retailer.MigrationService.DevDataSeed
{
	public class DataSeedManager
	{
		private readonly ProductFaker _productFaker;
		private readonly CustomerFaker _customerFaker;
		private readonly OrderFaker _ordersFaker;
		private readonly LineItemFaker _lineItemsFaker;
		private readonly ILogger<DataSeedManager> _logger;

		private readonly int _batchSize = 200;

		public DataSeedManager(ILogger<DataSeedManager> logger,int seed = 999)
		{
			_productFaker = new ProductFaker(seed);
			_customerFaker = new CustomerFaker(seed);
			_ordersFaker = new OrderFaker(seed);
			_lineItemsFaker = new LineItemFaker(seed);
			_logger = logger;
		}

		public async Task RunSeedOperationAsync(StoreDbContext context, SeedSize size = SeedSize.Small)
		{
			_logger.LogInformation("Starting data seed operation");
			(int productCount, int customerCount, int ordersCount, int lineItemsCount) = size switch
			{
				SeedSize.Small => (150, 300, 6000, 18_000),
				SeedSize.Medium => (300, 4500, 120_000, 490_000),
				SeedSize.Large => (500, 18_000, 550_000, 5_500_000),
				SeedSize.XXL => (900, 175_000, 9_300_000, 45_000_000),
				_ => throw new NotImplementedException("Unexpected value")
			};

			context.ChangeTracker.AutoDetectChangesEnabled = false;
			try
			{
				var productIds = await SeedProductsAsync(context, productCount);
				var customerIds = await SeedCustomersAsync(context, customerCount);
				var orderIds = await SeedOrdersAsync(context, ordersCount, customerIds);
				await SeedLineItemsAsync(context, lineItemsCount, customerIds, productIds);
			}
			catch(Exception exception)
			{
				_logger.LogError(exception, "Data seed operation error");
				context.ChangeTracker.AutoDetectChangesEnabled = true;
				throw;
			}
			finally
			{
				context.ChangeTracker.AutoDetectChangesEnabled = true;
			}
		}

		private async Task<List<int>> SeedProductsAsync(StoreDbContext context, int productCount)
		{
			_logger.LogInformation("Starting to seed products");
			var count = 0;
			List<int> productIds = new();

			while(count < productCount )
			{
				var batch = Math.Min(_batchSize, productCount - count);
				var items = _productFaker.GetFakeProducts(batch);
				context.AddRange(items);
				await context.SaveChangesAsync();
				context.ChangeTracker.Clear();
				productIds.AddRange(items.Select(x => x.Id));
				count += batch;				
			}

			_logger.LogInformation("Products seeding done");

			return productIds;
		}

		private async Task<List<int>> SeedCustomersAsync(StoreDbContext context, int customerCount)
		{
			_logger.LogInformation("Starting to seed customers");
			var count = 0;
			List<int> customerIds = new();

			while (count < customerCount)
			{
				var batch = Math.Min(_batchSize, customerCount - count);
				var items = _customerFaker.GetFakeCustomers(batch);
				context.AddRange(items);
				await context.SaveChangesAsync();
				context.ChangeTracker.Clear();
				customerIds.AddRange(items.Select(x => x.Id));
				count += batch;

				if((count % 1000) == 0)
				{
					_logger.LogDebug("{count} customers seeded", count);
				}
			}

			_logger.LogInformation("Customers seeding done");

			return customerIds;
		}

		private async Task<List<int>> SeedOrdersAsync(StoreDbContext context, int orderCount, IEnumerable<int> customerIds)
		{
			_logger.LogInformation("Starting to seed orders");

			var count = 0;
			List<int> orderIds = new();

			while (count < orderCount)
			{
				var batch = Math.Min(_batchSize, orderCount - count);
				var items = _ordersFaker.GetFakeOrders(customerIds, batch);
				context.AddRange(items);
				await context.SaveChangesAsync();
				context.ChangeTracker.Clear();
				orderIds.AddRange(items.Select(x => x.Id));
				count += batch;

				if ((count % 1000) == 0)
				{
					_logger.LogDebug("{count} orders seeded", count);
				}
			}

			_logger.LogInformation("Orders seeding done");

			return orderIds;
		}

		private async Task SeedLineItemsAsync(StoreDbContext context, int lineItemCount, IEnumerable<int> customerIds, IEnumerable<int> productIds)
		{
			_logger.LogInformation("Line items seeding starting");
			var count = 0;
			
			while (count < lineItemCount)
			{
				var batch = Math.Min(_batchSize, lineItemCount - count);
				var items = _lineItemsFaker.GetFakeLineItems(customerIds, productIds,batch);
				context.AddRange(items);
				await context.SaveChangesAsync();
				context.ChangeTracker.Clear();				
				count += batch;

				if ((count % 1000) == 0)
				{
					_logger.LogDebug("{count} line items seeded", count);
				}
			}
			_logger.LogInformation("Line items seeding done");
		}
	}
}
