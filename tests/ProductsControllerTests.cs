//using API.DataAccess;
//using API.DevDataSeed;
//using API.Models;
//using API.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Abstractions;
//using Microsoft.AspNetCore.Mvc.Routing;
//using Microsoft.AspNetCore.Routing;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace RetailApiTestProject
//{
//	public class ProductsControllerTests
//	{


//		private static List<Product> _productsSeed;
//		public ProductsControllerTests()
//		{

//			_productsSeed = new ProductFaker().GetFakeProducts(10);

//		}

//		private static ActionContext GetActionContextForPage(string page)
//		{
//			return new ActionContext()
//			{
//				ActionDescriptor = new ActionDescriptor()
//				{
//					RouteValues = new Dictionary<string, string>
//					{
//				{ "page", page },
//			}
//				},
//				RouteData = new RouteData()
//				{
//					Values =
//					{
//				[ "page" ] = page
//			}
//				}
//			};
//		}

//		[Fact]
//		public async Task GetAll_ShouldReturnAllProducts()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.GetAll_ShouldReturnAllProducts));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var controller = new ProductsController(null, productsDbcontext, null);

//			var result = await controller.GetAllAsync(new CancellationToken());

//			Assert.Equal(_productsSeed.Count(), result.Count());

//		}

//		[Fact]
//		public async Task GetById_ShouldReturnOkResult()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.GetById_ShouldReturnOkResult));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var controller = new ProductsController(null, productsDbcontext, null);

//			var result = await controller.GetByIdAsync(_productsSeed.Count, new CancellationToken());

//			Assert.IsType<OkObjectResult>(result.Result);

//		}

//		[Fact]
//		public async Task GetById_ShouldReturnNotFoundObjectResult()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.GetById_ShouldReturnNotFoundObjectResult));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var controller = new ProductsController(null, productsDbcontext, null);

//			var result = await controller.GetByIdAsync(_productsSeed.Count + 5, new CancellationToken());

//			Assert.IsType<NotFoundObjectResult>(result.Result);

//		}

//		[Fact]
//		public async Task GetById_ShouldReturnCorrespondingProduct()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.GetById_ShouldReturnCorrespondingProduct));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var controller = new ProductsController(null, productsDbcontext, null);

//			var result = await controller.GetByIdAsync(_productsSeed.Count, new CancellationToken());

//			Assert.IsType<OkObjectResult>(result.Result);
//			Assert.NotNull(result.Result);

//			Product product = null;
//			if (result.Result != null && result.Result is OkObjectResult)
//			{
//				product = (Product)((ObjectResult)result.Result).Value;
//				Assert.Equal(_productsSeed.Count, product.Id);
//			}
//		}

//		[Fact]
//		public async Task UpdateDescription_ShouldReturnNotFoundObjectResult()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.UpdateDescription_ShouldReturnNotFoundObjectResult));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var controller = new ProductsController(null, productsDbcontext, null);

//			var result = await controller.UpdateDescriptionAsync(_productsSeed.Count + 5, "test", new CancellationToken());

//			Assert.IsType<NotFoundObjectResult>(result.Result);

//		}

//		[Fact]
//		public async Task UpdateDescription_ShouldReturnOk()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.UpdateDescription_ShouldReturnOk));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var controller = new ProductsController(null, productsDbcontext, null);

//			var result = await controller.UpdateDescriptionAsync(_productsSeed.Count, "test", new CancellationToken());

//			Assert.IsType<OkObjectResult>(result.Result);

//		}

//		[Fact]
//		public async Task UpdateDescription_ShouldReturnBadRequestObjectResult()
//		{

//			var lengthLimit = typeof(Product)
//			   .GetProperty(nameof(Product.Description))
//			   .GetCustomAttributes(typeof(StringLengthAttribute), false)
//			   .OfType<StringLengthAttribute>()
//			   .FirstOrDefault()?
//			   .MaximumLength;

//			Assert.NotNull(lengthLimit);

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.UpdateDescription_ShouldReturnBadRequestObjectResult));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var controller = new ProductsController(null, productsDbcontext, null);

//			var result = await controller.UpdateDescriptionAsync(_productsSeed.Count, new string('a', lengthLimit.Value + 5), new CancellationToken());

//			Assert.IsType<BadRequestObjectResult>(result.Result);

//		}

//		[Fact]
//		public async Task UpdateDescription_ShouldUpdateDescriptionField()
//		{

//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.UpdateDescription_ShouldUpdateDescriptionField));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var guid = Guid.NewGuid();

//			var controller = new ProductsController(null, productsDbcontext, null);

//			var result = await controller.UpdateDescriptionAsync(_productsSeed.Count, guid.ToString(), new CancellationToken());


//			Assert.IsType<OkObjectResult>(result.Result);
//			Assert.NotNull(result.Result);

//			if (result.Result != null && result.Result is OkObjectResult)
//			{
//				var product = await productsDbcontext.Products.FindAsync(_productsSeed.Count);

//				var returnedProduct = (Product)((ObjectResult)result.Result).Value;
//				Assert.Equal(product.Id, returnedProduct.Id);
//				Assert.Equal(product.Description, returnedProduct.Description);
//				Assert.Equal(guid.ToString(), returnedProduct.Description);
//			}
//		}

//		[Fact]
//		public async Task GetAll20_ShouldReturnOkObject()
//		{

//			//Arrange
//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.GetAll20_ShouldReturnOkObject));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var context = GetActionContextForPage("/api/products");

//			var uriGeneratorMock = new Mock<IUriGenerator>();
//			uriGeneratorMock.Setup(x =>
//			 x.GeneratePaginationLinks(It.IsAny<PaginatedResponseModel<Product>>(), It.IsAny<string>()))
//			.Returns(new Dictionary<string, string> { { "Next", "xyz" } });

//			var urlHelperMock = new Mock<IUrlHelper>();
//			urlHelperMock.SetupGet(x => x.ActionContext)
//				.Returns(context);

//			urlHelperMock.Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
//				.Returns("api/products");

//			var ctx = new DefaultHttpContext();

//			var controller_sut = new ProductsController(null, productsDbcontext, uriGeneratorMock.Object);
//			controller_sut.ControllerContext = new ControllerContext() { HttpContext = ctx };
//			controller_sut.Url = urlHelperMock.Object;

//			//Act
//			var response = await controller_sut.GetAllAsync20(new Pagination(), new CancellationToken());

//			//Assert
//			Assert.IsType<OkObjectResult>(response.Result);

//		}

//		[Fact]
//		public async Task GetAll20_ShouldReturnNotFoundObjectResult()
//		{

//			//Arrange
//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.GetAll20_ShouldReturnNotFoundObjectResult));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var context = GetActionContextForPage("/api/products");

//			var uriGeneratorMock = new Mock<IUriGenerator>();
//			uriGeneratorMock.Setup(x =>
//			 x.GeneratePaginationLinks(It.IsAny<PaginatedResponseModel<Product>>(), It.IsAny<string>()))
//			.Returns(new Dictionary<string, string> { { "Next", "xyz" } });

//			var urlHelperMock = new Mock<IUrlHelper>();
//			urlHelperMock.SetupGet(x => x.ActionContext)
//				.Returns(context);

//			urlHelperMock.Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
//				.Returns("api/products");

//			var ctx = new DefaultHttpContext();

//			var controller_sut = new ProductsController(null, productsDbcontext, uriGeneratorMock.Object);
//			controller_sut.ControllerContext = new ControllerContext() { HttpContext = ctx };
//			controller_sut.Url = urlHelperMock.Object;


//			//Act
//			var response = await controller_sut.GetAllAsync20(new Pagination(10, 20), new CancellationToken());

//			//Assert
//			Assert.IsType<NotFoundObjectResult>(response.Result);

//		}

//		[Fact]
//		public async Task GetAll20_ShouldReturnBadRequestObjectResult()
//		{

//			//Arrange
//			var productsDbcontext = SetupInMemoryDbContext.GetProductsDbContext(nameof(this.GetAll20_ShouldReturnBadRequestObjectResult));
//			productsDbcontext.SeedAppDbContext(_productsSeed);

//			var context = GetActionContextForPage("/api/products");

//			var uriGeneratorMock = new Mock<IUriGenerator>();
//			uriGeneratorMock.Setup(x =>
//			 x.GeneratePaginationLinks(It.IsAny<PaginatedResponseModel<Product>>(), It.IsAny<string>()))
//			.Returns(new Dictionary<string, string> { { "Next", "xyz" } });

//			var urlHelperMock = new Mock<IUrlHelper>();
//			urlHelperMock.SetupGet(x => x.ActionContext)
//				.Returns(context);

//			urlHelperMock.Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
//				.Returns("api/products");

//			var ctx = new DefaultHttpContext();

//			var controller_sut = new ProductsController(null, productsDbcontext, uriGeneratorMock.Object);
//			controller_sut.ControllerContext = new ControllerContext() { HttpContext = ctx };
//			controller_sut.Url = urlHelperMock.Object;


//			//Act
//			var response = await controller_sut.GetAllAsync20(new Pagination(10, -20), new CancellationToken());

//			//Assert
//			Assert.IsType<BadRequestObjectResult>(response.Result);

//		}
//	}
//}
