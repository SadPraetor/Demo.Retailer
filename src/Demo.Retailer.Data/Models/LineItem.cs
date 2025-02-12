namespace Demo.Retailer.Data
{
	public record LineItem(int OrderId, int ProductId, int Quantity)
	{
		public int Id { get; private set; }

		public Order Order { get; set; } = default!;

		public Product Product { get; set; } = default!;
	}
}
