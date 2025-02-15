using Bogus;

namespace Demo.Retailer.MigrationService.DevDataSeed
{
	public class OrderFaker
	{
		private readonly Faker<Order> _faker;
		public OrderFaker(int useSeed = 999)
		{
			_faker = new Faker<Order>().UseSeed(useSeed);
		}

		public List<Order> GetFakeOrders(IEnumerable<int> customerIds, int count = 1)
		{
			if(customerIds is null || 
				!customerIds.TryGetNonEnumeratedCount(out count) || 
				count == 0)
			{
				throw new InvalidOperationException("Customer ids list is empty");
			}
			_faker.RuleFor(order => order.CustomerId, f => f.PickRandom<int>(customerIds));
			return _faker.Generate(count);
			
		}
	}
}
