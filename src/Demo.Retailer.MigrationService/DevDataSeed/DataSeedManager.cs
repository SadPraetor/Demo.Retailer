using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Retailer.MigrationService.DevDataSeed
{
	public class DataSeedManager
	{
		public DataSeedManager(int seed = 999)
		{
			var _productFaker = new ProductFaker(seed);

		}

		public async Task RunSeedOperation(StoreDbContext context, SeedSize size = SeedSize.Small)
		{

		}
	}
}
