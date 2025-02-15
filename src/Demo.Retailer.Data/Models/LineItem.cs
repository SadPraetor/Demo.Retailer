namespace Demo.Retailer.Data
{
	public record LineItem
	{
		public int OrderId { get; init; }
		public int ProductId { get; init; }
		public int Quantity { get; init; }
		public int Id { get; private set; }

		public Order Order { get; set; } = default!;

		public Product Product { get; set; } = default!;
	}
}
