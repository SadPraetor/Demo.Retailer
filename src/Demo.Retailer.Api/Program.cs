using Demo.Retailer.Api.Endpoints.Products;
using Demo.Retailer.Api.ExceptionHandlers;
using Demo.Retailer.Api.Infrastructure;
using Demo.Retailer.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using ApiVersion = Asp.Versioning.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
	.AddProjectApiVersions()
	.RegisterOpenApi();

builder.Services.AddDbContext<StoreDbContext>(options =>
				 options.UseSqlServer(builder.Configuration.GetConnectionString("DemoRetailer"))
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
	.AddOutputCache(options =>
	{
		options.AddPolicy(CachePolicies.id, builder =>
		{
			builder.AddPolicy<CachePolicyTagId>();
		});
		options.AddPolicy(CachePolicies.query, builder =>
		{
			builder.AddPolicy<CachePolicyQueryTagId>();
		});
	});


var app = builder.Build();

app.MapDefaultEndpoints();



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