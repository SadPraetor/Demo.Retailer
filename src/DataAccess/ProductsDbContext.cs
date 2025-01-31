using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess
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
