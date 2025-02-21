using Demo.Retailer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Demo.Retailer.OData.Controllers
{
	[Route("odata/[controller]")]
	public class ProductsController : ODataController
	{
		private readonly StoreDbContext _context;

		public ProductsController(StoreDbContext context)
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
