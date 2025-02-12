using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Demo.Retailer.Data
{
	public class StoreContextDesignFactory : IDesignTimeDbContextFactory<StoreDbContext>
	{
		public StoreDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<StoreDbContext>();
			builder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DemoRetailer;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
			return new StoreDbContext(builder.Options);
		}
	}
}
