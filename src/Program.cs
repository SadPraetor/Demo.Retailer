using API.DataAccess;
using API.DevDataSeed;
using API.Endpoints.Products;
using API.ExceptionHandlers;
using API.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using ApiVersion = Asp.Versioning.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");



builder.Services.AddDbContext<ProductsDbContext>(options =>
				 options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDb"))				 
			);

builder.Services.AddScoped<IUriGenerator, UriGenerator>();
builder.Services.AddExceptionHandler<FaultyPaginationQueryExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1);
	options.ReportApiVersions = true;
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(o=>
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

app.MapOpenApi();
app.UseSwaggerUI(
			options => {
//var service = context.ApplicationServices.GetService<IApiVersionDescriptionProvider>();	
				// build a swagger endpoint for each discovered API version
				//var service = app.Services.GetRequiredService<IActionDescriptorCollectionProvider>();
				
				//foreach (var description in service.ActionDescriptors.Items)
				//{
				//}
				
				options.SwaggerEndpoint($"/openapi/v1.json", "Demo.API.Retailer.V1");
				options.SwaggerEndpoint($"/openapi/v2.json", "Demo.API.Retailer.V2");
			});

app.UseExceptionHandler();


app.MapGet("/test", () => "HELLO THERE!");

var productsGroup = app.MapGroup("/api/v{version:apiVersion}/products")
	.WithApiVersionSet(app.NewApiVersionSet()
	.HasDeprecatedApiVersion(new ApiVersion(1))
	.HasApiVersion(new ApiVersion(2))
	.Build());

productsGroup.RegisterProductsAPIEndpoints();



app.Run();