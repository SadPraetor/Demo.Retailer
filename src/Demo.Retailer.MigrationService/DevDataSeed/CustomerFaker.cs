using Bogus;

namespace Demo.Retailer.MigrationService.DevDataSeed
{
	public class CustomerFaker
	{
		private readonly Faker<Customer> _faker;
		public CustomerFaker(int useSeed = 999)
		{
			_faker = new Faker<Customer>().UseSeed(useSeed)
				.RuleFor(c => c.FirstName, f => f.Name.FirstName())
				.RuleFor(c => c.LastName, f => f.Name.LastName());			
		}

		public List<Customer > GetFakeCustomers(int count=1)
		{
			return _faker.Generate(count);
		}
	}
}
