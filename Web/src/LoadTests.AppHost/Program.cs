var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Controlling>("Controlling");
builder.AddProject<Projects.Minimal>("Minimal");

builder.Build().Run();
