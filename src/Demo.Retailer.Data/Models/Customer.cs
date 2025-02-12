namespace Demo.Retailer.Data
{
	public class Customer
	{
		public int Id { get; set; }

		public string FirstName { get; set; } = default!;

		public string LastName { get; set; } = default!;

		public List<Order>? Orders { get; set; }
	}
}
