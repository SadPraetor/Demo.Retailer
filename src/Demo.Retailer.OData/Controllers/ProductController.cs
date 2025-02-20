using Demo.Retailer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Demo.Retailer.OData.Controllers
{
	[Route("odata/products")]
	public class ProductController : ODataController
	{
		private readonly StoreDbContext _context;

		public ProductController(StoreDbContext context)
		{
			_context = context;
		}

		[EnableQuery]
		public ActionResult<IQueryable<Product>> Get()
		{
			return _context.Products;
		}
	}
}
