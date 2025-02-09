using Demo.Retailer.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Retailer.Api.DataAccess
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
