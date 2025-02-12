namespace Demo.Retailer.Data
{
	public record Order(int CustomerId)
	{
		public int Id { get; private set; }
		public DateTime CreatedDate { get; private set; }

		public List<LineItem>? LineItems { get; set; }

		public Customer? Customer { get; set; }
	}
}
