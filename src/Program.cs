using API.DataAccess;
using API.DevDataSeed;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductsDbContext>(options =>
				 options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDb"))				 
			);


var app = builder.Build();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
{
	await app.MigrateDatabaseAsync();
	await app.SeedDatabaseIfEmptyAsync(3000);
}

app.MapGet("/test", () => "HELLO THERE!");

var productsGroup = app.MapGroup("/products");

productsGroup.MapGet("/", async (ProductsDbContext context, CancellationToken cancellationToken) =>
{
	return await context.Products.AsNoTracking().ToListAsync(cancellationToken);
});

app.Run();