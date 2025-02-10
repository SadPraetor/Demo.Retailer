using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Retailer.Data.Models
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.ToTable(nameof(StoreDbContext.Products));
			builder.HasKey(x => x.Id);

			builder.Property(x => x.Name)
				.IsRequired()
				.HasMaxLength(256);

			builder.Property(x => x.ImgUri)
				.IsRequired()
				.HasMaxLength(2083);

			builder.Property(x => x.Price)
				.IsRequired()
				.HasPrecision(18, 2);

			builder.Property(x => x.Description)
				.HasMaxLength(4000);


		}
	}
}
