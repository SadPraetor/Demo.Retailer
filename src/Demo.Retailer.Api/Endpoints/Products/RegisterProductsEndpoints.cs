using Demo.Retailer.Api.Infrastructure;
using Demo.Retailer.Api.Models;
using Demo.Retailer.Api.Services;
using Demo.Retailer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiVersion = Asp.Versioning.ApiVersion;

namespace Demo.Retailer.Api.Endpoints.Products
{
	public static class RegisterProductsEndpoints
	{
		public static RouteGroupBuilder RegisterProductsAPIEndpoints(this RouteGroupBuilder productsGroup)
		{
			productsGroup.MapGet("/", async (ProductsDbContext context, CancellationToken cancellationToken) =>
			{
				return TypedResults.Ok(await context.Products.AsNoTracking().ToListAsync(cancellationToken));
			})
			.MapToApiVersion(new ApiVersion(1))
			.WithTags("API");

			productsGroup.MapGet("/", GetProductsWithPagination)
				.MapToApiVersion(new ApiVersion(2))
				.WithTags("API")
				.WithName("paginated_products")
				.CacheOutput(CachePolicies.query);

			productsGroup.MapGet("/{id:int:min(1)}", async Task<Results<Ok<Product>, NotFound<ProblemDetails>>> (int id, ProductsDbContext context, CancellationToken cancellationToken) =>
			{
				var product = await context.Products.FindAsync(new object[] { id }, cancellationToken);

				if (product == null)
				{
					return TypedResults.NotFound(new ProblemDetails
					{
						Title = "Record not found",
						Detail = "Requested ID was not found",
						Status = StatusCodes.Status404NotFound
					});
				}

				return TypedResults.Ok(product);
			})
				.CacheOutput(CachePolicies.id);



			productsGroup.MapPatch("/{id:int:min(1)}", async (
				int id,
				PatchProductDto dto,
				ProductsDbContext context,
				IOutputCacheStore cache,
				CancellationToken cancellationToken) =>
				{
					var result = await UpdateDescriptionAsync(id, dto.Description, context, cancellationToken);
					await cache.EvictByTagAsync($"tag-id-{id}", cancellationToken);
					return result;
				})
				.Accepts<string>("application/json");

			productsGroup.MapPatch("/{id:int:min(1)}/description", async (
				int id,
				HttpRequest request,
				ProductsDbContext context,
				IOutputCacheStore cache,
				CancellationToken cancellationToken) =>
			{
				using var reader = new StreamReader(request.Body);
				var newDescription = await reader.ReadToEndAsync();

				var result = await UpdateDescriptionAsync(id, newDescription, context, cancellationToken);
				await cache.EvictByTagAsync($"tag-id-{id}", cancellationToken);
				return result;
			})
				.Accepts<string>("application/json");

			return productsGroup;
		}




		static async Task<Results<Ok<PaginatedResponseModel<Product>>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> GetProductsWithPagination(
			Pagination pagination,
			ProductsDbContext context,
			HttpContext httpContext,
			[FromServices] IUriGenerator uriGenerator,
			CancellationToken cancellationToken)
		{
			if (!pagination.IsValid)
			{
				return TypedResults.BadRequest(new ProblemDetails()
				{
					Title = "Faulty pagination filter",
					Detail = "Either page or size filter parameter is of wrong format",
					Extensions = { ["FailedValidations"] = pagination.ValidationMessage }
				});
			}

			try
			{
				var paginatedModel = await context
					.Products
					.AsNoTracking()
					.OrderBy(x => x.Id)
					.PaginateAsync<Product>(pagination.Page, pagination.Size, cancellationToken);

				paginatedModel.Links = uriGenerator.GeneratePaginationLinks<Product>(paginatedModel, httpContext);

				var tags = paginatedModel.Data.Select(x => $"tag-id-{x.Id}").ToArray();
				httpContext.Items.Add("cache-tag-ids", tags);

				return TypedResults.Ok(paginatedModel);

			}
			catch (PageOutOfRangeException exception)
			{
				return TypedResults.NotFound(new ProblemDetails()
				{
					Detail = exception.Message,
					Status = StatusCodes.Status404NotFound,
					Title = "Faulty pagination filter"
				});
			}
			catch (Exception)
			{
				return TypedResults.InternalServerError<ProblemDetails>(new ProblemDetails()
				{
					Title = "Internal server error",
					Detail = "Internal server error",
					Status = StatusCodes.Status500InternalServerError
				});
			}

		}

		private static readonly int _descriptionLengthLimit = typeof(Product)
				.GetProperty(nameof(Product.Description))
				.GetCustomAttributes(typeof(StringLengthAttribute), false)
				.OfType<StringLengthAttribute>()
				.FirstOrDefault()?
				.MaximumLength ?? int.MaxValue;

		private static async Task<Results<Ok<Product>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> UpdateDescriptionAsync(
			int id,
			string newDescription,
			ProductsDbContext context,
			CancellationToken cancellationToken)
		{
			if (newDescription.Length > _descriptionLengthLimit)
			{
				return TypedResults.BadRequest(new ProblemDetails()
				{
					Title = "Description exceeds length limit",
					Detail = $"New description length {newDescription.Length} exceeds field length limit",
					Status = StatusCodes.Status400BadRequest,
				});
			}

			var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			if (product == null)
			{
				return TypedResults.NotFound(new ProblemDetails()
				{
					Title = "Record not found",
					Detail = $"Requested ID {id} was not found",
					Status = StatusCodes.Status404NotFound
				});
			}

			product.Description = newDescription;

			await context.SaveChangesAsync();

			return TypedResults.Ok(product);
		}

	}
}
