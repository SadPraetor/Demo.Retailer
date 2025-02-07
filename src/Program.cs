using API.DataAccess;
using API.DevDataSeed;
using API.Endpoints.Products;
using API.ExceptionHandlers;
using API.Infrastructure;
using API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using System;
using ApiVersion = Asp.Versioning.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddProjectApiVersions()
	.RegisterOpenApi();

builder.Services.AddDbContext<ProductsDbContext>(options =>
				 options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDb"))
			);

builder.Services.AddScoped<IUriGenerator, UriGenerator>();

builder.Services
	.AddExceptionHandler<FaultyPaginationQueryExceptionHandler>()
	.AddProblemDetails()
	.Configure<ForwardedHeadersOptions>(options =>
	{
		options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
	});

builder.Services
	.AddOutputCache(options=>
	{
		options.AddPolicy(CachePolicies.id, builder =>
		{
			builder.AddPolicy<CachePolicyTagId>();
		});
	});


var app = builder.Build();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
{
	await app.MigrateDatabaseAsync();
	await app.SeedDatabaseIfEmptyAsync(3000);
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseOutputCache();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
	options.WithTheme(ScalarTheme.Moon);
});


app.UseExceptionHandler();


var productsGroup = app.MapGroup("/api/v{version:apiVersion}/products")
	.WithApiVersionSet(app.NewApiVersionSet()
	.HasDeprecatedApiVersion(new ApiVersion(1))
	.HasApiVersion(new ApiVersion(2))
	.Build());

productsGroup.RegisterProductsAPIEndpoints();

app.Run();

public partial class Program { } //for testing