using Bogus;

namespace Demo.Retailer.MigrationService.DevDataSeed
{
	public class OrderFaker
	{
		private Faker<Order> _faker;
		public OrderFaker(int useSeed = 999)
		{
			_faker = new Faker<Order>().UseSeed(useSeed)
				.RuleFor(o => o.CustomerId, _delegateMethod);
		}

		
		Func<Faker, int>? _delegateMethod;

		public List<Order> GetFakeOrders(IEnumerable<int> customerIds, int count = 1)
		{
			if(customerIds is null || 
				!customerIds.TryGetNonEnumeratedCount(out count) || 
				count == 0)
			{
				throw new InvalidOperationException("Customer ids list is empty");
			}
			_delegateMethod = (Faker faker) => faker.PickRandom(customerIds);
			return _faker.Generate(count);
			
		}
	}
}
