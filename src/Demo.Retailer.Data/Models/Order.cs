namespace Demo.Retailer.Data
{
	public record Order
	{
		public int Id { get; private set; }
		public int CustomerId { get; init; }
		public DateTime CreatedDate { get; private set; }

		public List<LineItem>? LineItems { get; set; }

		public Customer? Customer { get; set; }
	}
}
