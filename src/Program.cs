using API.DataAccess;
using API.DevDataSeed;
using API.Endpoints.Products;
using API.ExceptionHandlers;
using API.Models;
using API.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ApiVersion = Asp.Versioning.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2", options =>
{
	options.AddOperationTransformer((operation, context, cancellationToken) =>
	{
		if (context.Description.ActionDescriptor.EndpointMetadata
		.OfType<IParameterBindingMetadata>()
		.Any(x => x.ParameterInfo.ParameterType == typeof(Pagination)))
		{
			operation.Parameters ??= [];
			operation.Parameters.Add(new OpenApiParameter()
			{
				Name = nameof(Pagination.Page).ToLower(),
				In = ParameterLocation.Query,
				Required = false,
				Schema = new OpenApiSchema() { Type = "integer", Minimum = 1, Format = "int32" }

			});
			operation.Parameters.Add(new OpenApiParameter()
			{
				Name = nameof(Pagination.Size).ToLower(),
				In = ParameterLocation.Query,
				Required = false,
				Schema = new OpenApiSchema() { Type = "integer", Format = "int32", Description = "Items to retrieve. Limited to maximum 100", Maximum = 100, Minimum = 1 }
			});
		}
		return Task.CompletedTask;
	});
});



builder.Services.AddDbContext<ProductsDbContext>(options =>
				 options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDb"))
			);

builder.Services.AddScoped<IUriGenerator, UriGenerator>();
builder.Services.AddExceptionHandler<FaultyPaginationQueryExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
});

builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1);
	options.ReportApiVersions = true;
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(o =>
{
	o.GroupNameFormat = "'v'VVV";
	o.SubstituteApiVersionInUrl = true;
});


var app = builder.Build();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
{
	await app.MigrateDatabaseAsync();
	await app.SeedDatabaseIfEmptyAsync(3000);
}

app.UseForwardedHeaders();

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