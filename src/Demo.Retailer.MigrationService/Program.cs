global using Demo.Retailer.Data;
using Demo.Retailer.MigrationService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.AddSqlServerDbContext<StoreDbContext>("ValuesChangedMonitoring");


var host = builder.Build();
host.Run();
