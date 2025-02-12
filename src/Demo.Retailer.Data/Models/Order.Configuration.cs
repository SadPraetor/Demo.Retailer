using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Retailer.Data
{
	public class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.ToTable(nameof(StoreDbContext.Orders));

			builder.HasKey(x => x.Id);
			builder.Property(x => x.Id)
				.UseIdentityColumn(1, 1);

			builder.HasIndex(order => order.CustomerId);

			builder.Property(x => x.CreatedDate)
				.HasDefaultValueSql("GetUtcDate()")
				.ValueGeneratedOnAdd();

			builder.HasMany(order => order.LineItems)
				.WithOne(lineItem => lineItem.Order)
				.HasPrincipalKey(order => order.Id)
				.HasForeignKey(lineItem => lineItem.OrderId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
