using System.ComponentModel.DataAnnotations;

namespace Demo.Retailer.Data
{

	public class Product
	{
		public int Id { get; set; }

		public string Name { get; set; }

		[DataType(DataType.ImageUrl)]
		public string ImgUri { get; set; }


		[DataType(DataType.Currency)]
		public decimal Price { get; set; }

		public string? Description { get; set; }

		public bool InStock { get; set; }
	}
}
