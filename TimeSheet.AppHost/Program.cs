using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var timeSheetSqlServer = builder.AddSqlServer("timeSheetSqlServer", port: 6033)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(port: 6033, targetPort: 1433, name: "TimeSheet")
    .WithContainerName("timeSheetSqlServer")
    .AddDatabase("TimeSheet");

var authSqlServer = builder.AddSqlServer("authSqlServer", port: 6034)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(port: 6034, targetPort: 1433, name: "Auth")
    .WithContainerName("authSqlServer")
    .AddDatabase("Auth");

var apiService = builder.AddProject<Projects.TimeSheet_ApiService>("timesheet-api")
    .WithReference(timeSheetSqlServer)
    .WaitFor(timeSheetSqlServer);

var authService = builder.AddProject<Projects.TimeSheet_AuthAPI>("auth-api")
    .WithReference(authSqlServer)
    .WaitFor(authSqlServer);

var apiGateway = builder.AddProject<Projects.TimeSheet_ApiGateway>("gateway-api")
    .WithReference(apiService);

builder.AddProject<Projects.TimeSheet_Web>("frontend-app")
    .WithExternalHttpEndpoints()
    .WithReference(apiGateway)
    .WithReference(authService);

builder.Build().Run();
