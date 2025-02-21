using Demo.Retailer.Data;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<StoreDbContext>(options =>
				 options.UseSqlServer(builder.Configuration.GetConnectionString("DemoRetailer"))
			);
// Add services to the container.

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Product>("Products");


builder.Services.AddControllers()
    .AddOData(options=>
    {

        options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()    //headsup, if entity set is "Products", also controller name must be "ProductsController". Just "ProductController" wont work for /Products/$count
        .SetMaxTop(100)
        .AddRouteComponents("odata", modelBuilder.GetEdmModel());        
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseODataQueryRequest();

app.Run();
