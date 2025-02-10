using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Retailer.Data.Models
{
	public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
	{
		public void Configure(EntityTypeBuilder<Customer> builder)
		{
			builder.ToTable(nameof(StoreDbContext.Customers));

			builder.HasKey(x => x.Id);
			builder.Property(x => x.Id)
				.UseIdentityColumn(1, 1);

			builder.Property(x => x.FirstName)
				.HasMaxLength(256);

			builder.Property(x => x.LastName)
				.HasMaxLength(256);				
		}
	}
}
