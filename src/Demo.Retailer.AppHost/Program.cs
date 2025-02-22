using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlServer("demo-retailer-sql", port: 1433)
	.WithImageTag("2022-CU16-ubuntu-22.04")
	.WithContainerName("demo-retailer")
	.WithEnvironment("ACCEPT_EULA", "Y")
	.WithEnvironment("TrustServerCertificate", "True")
	.WithEnvironment("Encrypt", "True")	
	.WithLifetime(ContainerLifetime.Persistent)
	.PublishAsConnectionString()
	.AddDatabase("DemoRetailer");

var migration = builder.AddProject<Projects.Demo_Retailer_MigrationService>("demo-retailer-migration")
	.WithEnvironment("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
	.WithReference(db)
	.WaitFor(db);


builder.AddProject<Projects.Demo_Retailer_Api>("demo-retailer-api")
	.WithEnvironment("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
	.WithReference(db)
	.WaitFor(db)
	.WaitForCompletion(migration);

builder.AddProject<Projects.Demo_Retailer_OData>("demo-retailer-odata")
	.WithEnvironment("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
	.WithReference(db)
	.WaitFor(db)
	.WaitForCompletion(migration); ;

builder.Build().Run();
