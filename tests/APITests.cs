using API.DataAccess;
using API.DevDataSeed;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StronglyTypedId.Tests;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
	[Collection(nameof(ProductsTestsCollection))]
	public class APITests : IDisposable
	{
		protected readonly DbContainerFixture _fixture;
		private readonly ITestOutputHelper _output;
		private readonly ProductsDbContext _dbContext;
		protected SqlConnection GetSqlConnection() => _fixture.GetConnection();
		protected ProductsDbContext CreateDbContext() => new ProductsDbContext(new DbContextOptionsBuilder<ProductsDbContext>()
			.UseSqlServer(_fixture.GetConnection())
			.LogTo(_output.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information).Options);

		public APITests(DbContainerFixture fixture, ITestOutputHelper output)
		{
			_fixture = fixture;
			_output = output;
			_dbContext = new ProductsDbContext(new DbContextOptionsBuilder<ProductsDbContext>()
			.UseSqlServer(_fixture.GetConnection())
				.Options);

			_dbContext.Database.Migrate();

			var faker = new ProductFaker();
			var products = faker.GetFakeProducts(300);
			_dbContext.Products.AddRange(products);
			_dbContext.SaveChanges();

		}
		public void Dispose()
		{
			_dbContext.Database.EnsureDeleted();
			_dbContext.Dispose();
		}

		[Fact]
		public async Task SetupSeedsDB()
		{
			var context = CreateDbContext();

			var products = await context.Products.ToListAsync();

			Assert.True(products.Count == 300);

		}
	}
}
