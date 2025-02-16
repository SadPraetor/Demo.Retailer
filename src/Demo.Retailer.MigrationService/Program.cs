global using Demo.Retailer.Data;
using Demo.Retailer.MigrationService;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<StoreDbContext>("DemoRetailer");


var host = builder.Build();
host.Run();
