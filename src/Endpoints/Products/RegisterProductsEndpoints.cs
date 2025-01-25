using API.DataAccess;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiVersion = Asp.Versioning.ApiVersion;

namespace API.Endpoints.Products
{
	public static class RegisterProductsEndpoints
	{
		public static RouteGroupBuilder RegisterProductsAPIEndpoints(this RouteGroupBuilder productsGroup)
		{
			productsGroup.MapGet("/", async (ProductsDbContext context, CancellationToken cancellationToken) =>
			{
				return await context.Products.AsNoTracking().ToListAsync(cancellationToken);
			})
			.MapToApiVersion(new ApiVersion(1));

			productsGroup.MapGet("/", GetProductsWithPagination)
				.MapToApiVersion(new ApiVersion(2));

			return productsGroup;
		}




		static async Task<Results<Ok<PaginatedResponseModel<Product>>, NotFound<ProblemDetails>,InternalServerError<ProblemDetails>>> GetProductsWithPagination(
			Pagination pagination,
			ProductsDbContext context,
			HttpContext httpContext,
			[FromServices] IUriGenerator uriGenerator,
			CancellationToken cancellationToken)
		{
			try
			{

				var paginationFilter = new PaginationFilter(pagination);

				var paginatedModel = await context
					.Products
					.AsNoTracking()
					.OrderBy(x => x.Id)
					.PaginateAsync<Product>(paginationFilter.Page, paginationFilter.Size, cancellationToken);

				var path = new Uri(httpContext.Request.GetEncodedUrl()).GetLeftPart(UriPartial.Path).ToString();

				paginatedModel.Links = uriGenerator.GeneratePaginationLinks<Product>(paginatedModel, path);

				return TypedResults.Ok(paginatedModel);

			}
			catch (PageOutOfRangeException exception)
			{
				return TypedResults.NotFound(new ProblemDetails()
				{
					Detail = "Page requested is out of range",
					Status = StatusCodes.Status404NotFound,
					Title = "Faulty pagination filter"
				});
			}			
			catch (Exception exception)
			{
				return TypedResults.InternalServerError<ProblemDetails>(new ProblemDetails()
				{
					Title = "Internal server error",
					Detail = "Internal server error",
					Status = StatusCodes.Status500InternalServerError
				});
			}
			
		}
	}
}
