using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Retailer.Data.Models
{
	public class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
	{
		public void Configure(EntityTypeBuilder<LineItem> builder)
		{
			builder.ToTable(nameof(StoreDbContext.LineItems));

			builder.HasIndex(lineItem => lineItem.OrderId);

			builder.HasOne(lineItem => lineItem.Product)
				.WithMany()
				.HasForeignKey(lineItem => lineItem.ProductId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
