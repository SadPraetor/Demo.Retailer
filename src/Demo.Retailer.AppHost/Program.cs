var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Demo_Retailer_Api>("api");

builder.Build().Run();
