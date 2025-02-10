using Demo.Retailer.Api.Models;
using Demo.Retailer.Data;
using Demo.Retailer.MigrationService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Tests.APITests
{
	public static class ModuleInitializer
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			VerifyHttp.Initialize();
			Verifier.UseSourceFileRelativeDirectory("snapshosts");
		}
	}

	[Collection(nameof(ProductsTestsCollection))]
	public class APITests : IDisposable
	{
		protected readonly APITestsFixture _fixture;
		private readonly ITestOutputHelper _output;
		private readonly StoreDbContext _dbContext;

		protected SqlConnection GetSqlConnection() => _fixture.GetConnection();
		protected StoreDbContext CreateDbContext() => new StoreDbContext(new DbContextOptionsBuilder<StoreDbContext>()
			.UseSqlServer(_fixture.GetConnection())
			.LogTo(_output.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information).Options);

		public APITests(APITestsFixture fixture, ITestOutputHelper output)
		{
			_fixture = fixture;
			_output = output;
			_dbContext = new StoreDbContext(new DbContextOptionsBuilder<StoreDbContext>()
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

			await Verifier.Verify(result);
		}

		[Fact]
		public async Task GetProductV1_ShouldReturnItem()
		{
			var client = _fixture.CreateClient();
			var context = CreateDbContext();

			var item = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);

			var result = await client.GetAsync("api/v1/products/25");

			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			await Verifier.Verify(result);

			var apiResponseItem = await result.Content.ReadFromJsonAsync<Product>();

			Assert.True(apiResponseItem.Id == item.Id);
			Assert.True(apiResponseItem.Description == item.Description);
		}

		[Fact]
		public async Task GetProductV2_ShouldReturnItem()
		{
			var client = _fixture.CreateClient();
			var context = CreateDbContext();

			var item = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);

			var result = await client.GetAsync("api/v2/products/25");

			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			await Verifier.Verify(result);

			var apiResponseItem = await result.Content.ReadFromJsonAsync<Product>();

			Assert.True(apiResponseItem.Id == item.Id);
			Assert.True(apiResponseItem.Description == item.Description);
		}

		[Fact]
		public async Task PathDescriptionV1_ShouldUpdateItemDescription()
		{
			var client = _fixture.CreateClient();
			var context = CreateDbContext();

			var originalItem = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);

			var content = new StringContent("new description", Encoding.UTF8, MediaTypeNames.Application.Json);

			var result = await client.PatchAsync("api/v1/products/25/description", content);
			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			context.ChangeTracker.Clear();
			var updatedItem = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);


			await Verifier.Verify(result);

			var apiResponseItem = await result.Content.ReadFromJsonAsync<Product>();

			Assert.True(apiResponseItem.Id == originalItem.Id);
			Assert.False(apiResponseItem.Description == originalItem.Description);

			Assert.True(apiResponseItem.Id == updatedItem.Id);
			Assert.True(apiResponseItem.Description == updatedItem.Description);
			Assert.True(apiResponseItem.Description == "new description");
		}

		[Fact]
		public async Task PathDescriptionV2_ShouldUpdateItemDescription()
		{
			var client = _fixture.CreateClient();
			var context = CreateDbContext();

			var originalItem = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);

			var content = new StringContent("new description", Encoding.UTF8, MediaTypeNames.Application.Json);

			var result = await client.PatchAsync("api/v1/products/25/description", content);
			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			context.ChangeTracker.Clear();
			var updatedItem = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);


			await Verifier.Verify(result);

			var apiResponseItem = await result.Content.ReadFromJsonAsync<Product>();

			Assert.True(apiResponseItem.Id == originalItem.Id);
			Assert.False(apiResponseItem.Description == originalItem.Description);

			Assert.True(apiResponseItem.Id == updatedItem.Id);
			Assert.True(apiResponseItem.Description == updatedItem.Description);
			Assert.True(apiResponseItem.Description == "new description");
		}

		[Fact]
		public async Task PathDescriptionDtoV1_ShouldUpdateItemDescription()
		{
			var client = _fixture.CreateClient();
			var context = CreateDbContext();

			var originalItem = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);


			var result = await client.PatchAsJsonAsync("api/v1/products/25/", new DescriptionDto("new description by dto"));
			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			context.ChangeTracker.Clear();
			var updatedItem = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);


			await Verifier.Verify(result);

			var apiResponseItem = await result.Content.ReadFromJsonAsync<Product>();

			Assert.True(apiResponseItem.Id == originalItem.Id);
			Assert.False(apiResponseItem.Description == originalItem.Description);

			Assert.True(apiResponseItem.Id == updatedItem.Id);
			Assert.True(apiResponseItem.Description == updatedItem.Description);
			Assert.True(apiResponseItem.Description == "new description by dto");
		}

		[Fact]
		public async Task PathDescriptionDtoV2_ShouldUpdateItemDescription()
		{
			var client = _fixture.CreateClient();
			var context = CreateDbContext();

			var originalItem = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);

			var result = await client.PatchAsJsonAsync("api/v1/products/25/", new DescriptionDto("new description by dto"));
			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			context.ChangeTracker.Clear();
			var updatedItem = await context.Products.FirstOrDefaultAsync(product => product.Id == 25);


			await Verifier.Verify(result);

			var apiResponseItem = await result.Content.ReadFromJsonAsync<Product>();

			Assert.True(apiResponseItem.Id == originalItem.Id);
			Assert.False(apiResponseItem.Description == originalItem.Description);

			Assert.True(apiResponseItem.Id == updatedItem.Id);
			Assert.True(apiResponseItem.Description == updatedItem.Description);
			Assert.True(apiResponseItem.Description == "new description by dto");
		}

		[Fact]
		public async Task GetProductV2_WithFilterPage2_ShouldReturnItems()
		{
			var client = _fixture.CreateClient();

			var result = await client.GetAsync("api/v2/products?page=2&size=10");

			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			await Verifier.Verify(result);
		}

		[Fact]
		public async Task GetProductV2_WithFilterLastPage_ShouldReturnItemsNoNextPage()
		{
			var client = _fixture.CreateClient();

			var result = await client.GetAsync("api/v2/products?page=30&size=10");

			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			await Verifier.Verify(result);
		}

		[Fact]
		public async Task GetProductV2_ForwardedHeaders()
		{
			var client = _fixture.CreateClient();

			var message = new HttpRequestMessage(HttpMethod.Get, "api/v2/products?page=3&size=10");
			message.Headers.Add("X-Forwarded-Host", "www.myDemo.Retailer.Api.com");
			message.Headers.Add("X-Forwarded-Proto", "https");

			var result = await client.SendAsync(message);



			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

			await Verifier.Verify(result);
		}
	}
}
