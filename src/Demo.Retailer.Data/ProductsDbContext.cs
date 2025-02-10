
using Microsoft.EntityFrameworkCore;

namespace Demo.Retailer.Data
{
	public class ProductsDbContext : DbContext
	{

		public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
			: base(options)
		{
		}

		public DbSet<Product> Products { get; set; }

	}
}
