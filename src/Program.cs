using API.DataAccess;
using API.DevDataSeed;
using API.Models;
using API.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using ApiVersion = Asp.Versioning.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductsDbContext>(options =>
				 options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDb"))				 
			);

builder.Services.AddScoped<IUriGenerator, UriGenerator>();

builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1);
	options.ReportApiVersions = true;
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
{
	await app.MigrateDatabaseAsync();
	await app.SeedDatabaseIfEmptyAsync(3000);
}

app.MapGet("/test", () => "HELLO THERE!");

var productsGroup = app.MapGroup("/api/v{version:apiVersion}/products")
	.WithApiVersionSet(app.NewApiVersionSet()
	.HasDeprecatedApiVersion(new ApiVersion(1))
	.HasApiVersion(new ApiVersion(2))
	.Build());

productsGroup.MapGet("/", async (ProductsDbContext context, CancellationToken cancellationToken) =>
{
	return await context.Products.AsNoTracking().ToListAsync(cancellationToken);
})
	.MapToApiVersion(new ApiVersion(1));

productsGroup.MapGet("/", async (
	Pagination pagination,
	ProductsDbContext context,
	HttpContext httpContext,
	[FromServices] IUriGenerator uriGenerator,
	CancellationToken cancellationToken) =>
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

		return Results.Ok(paginatedModel);

	}
	catch (PageOutOfRangeException exception)
	{
		return Results.NotFound(new ExceptionDto(exception));
	}
	catch (FaultyPaginationQueryException exception)
	{
		return Results.BadRequest(new ExceptionDto(exception));
	}
	catch (Exception exception)
	{
		return Results.InternalServerError<ExceptionDto>(new ExceptionDto(exception));		
	}
})
	.MapToApiVersion(new ApiVersion(2));

app.Run();