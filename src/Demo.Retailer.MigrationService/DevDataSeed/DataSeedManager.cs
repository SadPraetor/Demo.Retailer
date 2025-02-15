using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Retailer.MigrationService.DevDataSeed
{
	public class DataSeedManager
	{
		private readonly ProductFaker _productFaker;
		private readonly CustomerFaker _customerFaker;
		private readonly OrderFaker _ordersFaker;
		private readonly LineItemFaker _lineItemsFaker;

		public DataSeedManager(int seed = 999)
		{
			_productFaker = new ProductFaker(seed);
			_customerFaker = new CustomerFaker(seed);
			_ordersFaker = new OrderFaker(seed);
			_lineItemsFaker = new LineItemFaker(seed);
		}

		public async Task RunSeedOperation(StoreDbContext context, SeedSize size = SeedSize.Small)
		{
			(int productCount, int customerCount, int ordersCount, int lineItemsCount) = size switch
			{
				SeedSize.Small => (150, 300, 6000, 18_000),
				SeedSize.Medium => (300, 4500, 120_000, 490_000),
				SeedSize.Large => (500, 18_000, 550_000, 2_500_000),
				SeedSize.XXL => (900, 75_000, 1_300_000, 12_000_000),
			};


		}
	}
}
