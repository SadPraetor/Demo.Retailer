//using API.DevDataSeed;
//using API.Models;
//using API.Services;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace RetailApiTestProject
//{
//	public class UtilitiesTests
//	{

//		[Fact]
//		public async Task PaginateAsync_ShouldReturn10Products()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.PaginateAsync_ShouldReturn10Products));
//			productsDbcontext.SeedAppDbContext(new ProductFaker().GetFakeProducts(20));

//			var result = await productsDbcontext.Products.PaginateAsync(1, 10, new CancellationToken());

//			Assert.Equal(10, result.Data.Count);

//		}


//		[Theory]
//		[InlineData(1)]
//		[InlineData(25)]
//		[InlineData(50)]
//		public async Task PaginateAsync_ShouldReturnNProducts(int pageSize)
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.PaginateAsync_ShouldReturnNProducts));
//			productsDbcontext.SeedAppDbContext(new ProductFaker().GetFakeProducts(120));

//			var result = await productsDbcontext.Products.PaginateAsync(1, pageSize, new CancellationToken());

//			Assert.Equal(pageSize, result.Data.Count);

//		}


//		[Fact]
//		public async Task PaginateAsync_ShouldReturn2ndPage()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.PaginateAsync_ShouldReturn10Products));
//			productsDbcontext.SeedAppDbContext(new ProductFaker().GetFakeProducts(20));

//			var result = await productsDbcontext.Products.PaginateAsync(2, 10, new CancellationToken());

//			Assert.Equal(10, result.Data.Count);
//			Assert.Equal(11, result.Data.First().Id);
//			Assert.Equal(20, result.Data.Last().Id);
//			Assert.Equal(2, result.CurrentPage);

//		}
//		[Theory]
//		[InlineData(49, 1)]
//		[InlineData(25, 2)]
//		[InlineData(1, 50)]
//		public async Task PaginateAsync_ShouldCalcualteTotalPages(int expected, int pageSize)
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.PaginateAsync_ShouldCalcualteTotalPages) + pageSize.ToString());
//			productsDbcontext.SeedAppDbContext(new ProductFaker().GetFakeProducts(49));

//			var result = await productsDbcontext.Products.PaginateAsync(1, pageSize, new CancellationToken());

//			Assert.Equal(expected, result.TotalPages);
//		}

//		[Fact]
//		public void UriGenerator_ContainsNextDoesNotContainPrev()
//		{

//			var responseModel = new PaginatedResponseModel<Product>();
//			var path = "api/products";

//			responseModel.TotalPages = 10;
//			responseModel.CurrentPage = 1;

//			var generator = new UriGenerator("https://domain.com");

//			var links = generator.GeneratePaginationLinks(responseModel, path);

//			Assert.Contains<string>(LinkType.Next.ToString(), links.Select(x => x.Key));
//			Assert.DoesNotContain<string>(LinkType.Prev.ToString(), links.Select(x => x.Key));

//		}
//		[Fact]
//		public void UriGenerator_ContainsPrevDoesNotContainNext()
//		{

//			var responseModel = new PaginatedResponseModel<Product>();
//			var path = "api/products";

//			responseModel.TotalPages = 10;
//			responseModel.CurrentPage = 10;

//			var generator = new UriGenerator("https://domain.com");

//			var links = generator.GeneratePaginationLinks(responseModel, path);

//			Assert.Contains<string>(LinkType.Prev.ToString(), links.Select(x => x.Key));
//			Assert.DoesNotContain<string>(LinkType.Next.ToString(), links.Select(x => x.Key));

//		}

//		[Fact]
//		public void UriGenerator_ProperLinkGeneration()
//		{

//			var responseModel = new PaginatedResponseModel<Product>();
//			var path = "api/products";

//			responseModel.TotalPages = 10;
//			responseModel.CurrentPage = 5;
//			responseModel.PageSize = 10;

//			var generator = new UriGenerator("https://domain.com");

//			var links = generator.GeneratePaginationLinks(responseModel, path);

//			Assert.Equal("https://domain.com/api/products?pageSize=10&page=6", links["Next"], ignoreCase: true);
//			Assert.Equal("https://domain.com/api/products?pageSize=10&page=4", links["Prev"], ignoreCase: true);
//		}

//		[Fact]
//		public void PaginationQuery_ShouldDefaultPage1PageSize10()
//		{

//			var paginationFilter = new Pagination();

//			Assert.Equal(1, paginationFilter.Page);
//			Assert.Equal(10, paginationFilter.Size);

//		}

//		[Fact]
//		public void PaginationFilter_ShouldThrowException()
//		{
//			Assert.Throws<FaultyPaginationQueryException>(() => new PaginationFilter(-1, 10));
//			Assert.Throws<FaultyPaginationQueryException>(() => new PaginationFilter(1, -10));
//		}

//		[Fact]
//		public void PaginationFilter_LimitsTo100()
//		{
//			var sut = new PaginationFilter(150, 5);
//			Assert.Equal(100, sut.Size);
//		}
//	}
//}
