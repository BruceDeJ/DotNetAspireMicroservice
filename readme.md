# TimeSheet Application

The TimeSheet application is a demonstration application that aims to explore how you can setup microservices in .NET using .NET aspire. This application includes:

* A blazor web page
* A .NET Aspire host project
* A TimeSheet API 
* A Auth API
* A API Gateway (implemented using Ocelot)

## Installation

You will need the below SDKs/Tools to run this application

* [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* An OCI Compliant container runtime, such as [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* An IDE Editor such as [Visual Studio](https://visualstudio.microsoft.com/vs/)


You can find a detailed guide on .NET Aspire setup and tooling at this [link](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=windows&pivots=visual-studio).

## Usage

To run the application simply open the solution in Visual Studio and then run the "https" launch profile. Alternatively you can run the application from the CLI by navigating to the ./Timesheet.AppHost directory and then in CMD run `dotnet run`.



## License

[MIT](https://choosealicense.com/licenses/mit/)

Note to self, you need to run Rancher before the SQL docker containers will run