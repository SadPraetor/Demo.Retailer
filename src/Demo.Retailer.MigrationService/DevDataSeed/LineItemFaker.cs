using Bogus;

namespace Demo.Retailer.MigrationService.DevDataSeed
{
	public class LineItemFaker
	{
		private readonly Faker<LineItem> _faker;
		
		public LineItemFaker(int useSeed = 999)
		{
			_faker = new Faker<LineItem>().UseSeed(useSeed)
				
				.RuleFor(l => l.Quantity, f => f.Random.Int(1, 1500));
		}


		public List<LineItem> GetFakeLineItems(IEnumerable<int> orderIds,IEnumerable<int> productIds, int count = 1)
		{
			if (orderIds is null ||
				!orderIds.Any())
			{
				throw new InvalidOperationException("Order ids list is empty");
			}

			if (productIds is null ||
				!productIds.Any())
			{
				throw new InvalidOperationException("Product ids list is empty");
			}

			_faker.RuleFor(lineItem => lineItem.OrderId, f => f.PickRandom<int>(orderIds));
			_faker.RuleFor(lineItem => lineItem.ProductId, f => f.PickRandom<int>(productIds));
			return _faker.Generate(count);
		}
	}
}
