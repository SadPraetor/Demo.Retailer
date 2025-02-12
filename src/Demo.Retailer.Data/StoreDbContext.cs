
using Microsoft.EntityFrameworkCore;

namespace Demo.Retailer.Data
{
	public class StoreDbContext : DbContext
	{

		public StoreDbContext(DbContextOptions<StoreDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("store");

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);
		}

		public DbSet<Product> Products { get; set; }

		public DbSet<Customer> Customers { get; set; }

		public DbSet<Order> Orders { get; set; }

		public DbSet<LineItem> LineItems { get; set; }
	}
}
