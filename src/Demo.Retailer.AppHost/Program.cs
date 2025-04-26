using Aspire.Hosting;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

	

var elastic = builder.AddElasticsearch("demo-retailer-elastic")
	.WithImageTag("8.18.0")
	
	.WithEnvironment("discovery.type", "single-node")
	.WithEnvironment("xpack.security.enabled", "false")
	//.WithEnvironment("xpack.security.enrollment.enabled", "true")
	.WithDataVolume("demo-retailer-elastic-data")
	.WithLifetime(ContainerLifetime.Persistent)
	//.WithHttpEndpoint(9200,9200)
	.WithEndpoint(9200,9200)
	.PublishAsConnectionString();


builder.AddContainer("demo-retailer-kibana", "kibana", "8.18.0")
	.WithEnvironment("ELASTICSEARCH_HOSTS", "http://demo-retailer-elastic:9200")
	.WithEndpoint(5601, 5601)
	.WithReference(elastic)
	.WaitFor(elastic)
	.WithLifetime(ContainerLifetime.Persistent);


var db = builder.AddSqlServer("demo-retailer-sql", port: 1433)
	.WithImageTag("2022-CU16-ubuntu-22.04")	
	.WithContainerName("demo-retailer-sql")
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
