using API.DataAccess;
using API.DevDataSeed;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Tests.APITests
{
	[Collection(nameof(ProductsTestsCollection))]
	public class APITests : IDisposable
	{
		protected readonly APITestsFixture _fixture;
		private readonly ITestOutputHelper _output;
		private readonly ProductsDbContext _dbContext;
		
		protected SqlConnection GetSqlConnection() => _fixture.GetConnection();
		protected ProductsDbContext CreateDbContext() => new ProductsDbContext(new DbContextOptionsBuilder<ProductsDbContext>()
			.UseSqlServer(_fixture.GetConnection())
			.LogTo(_output.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information).Options);

		public APITests( APITestsFixture fixture, ITestOutputHelper output)
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

		[Fact]
		public async Task GetProductsV1_GetsAllProducts()
		{
			var client = _fixture.CreateClient();

			var result = await client.GetAsync("api/v1/products");

			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
		}
	}
}
