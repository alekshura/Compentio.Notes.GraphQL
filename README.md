# Compentio.Ferragosto.Notes

This is .NET 5 template for Microservice with Azure Active Dicrectory authorization and MongoDB underneath.

![GitHub top language](https://img.shields.io/github/languages/top/alekshura/Compentio.Ferragosto.Notes)
![GitHub contributors](https://img.shields.io/github/contributors/alekshura/Compentio.Ferragosto.Notes)


# Libraries 
## API
- Swagger - Swashbuckle.AspNetCore
- Logging: Application Insights provider: Azure monitor and Application Insights in code: https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger
- Serizalization used: `System.Text.Json`. Comparison with `Newtonsoft.Json` here: https://medium.com/takeaway-tech/json-serialization-libraries-performance-tests-b54cbb3cccbb
- SonarAnalyzer.CSharp - static code analysis to be performed locally: https://www.nuget.org/packages/SonarAnalyzer.CSharp/. More: https://arminreiter.com/2016/08/use-code-analyzers-csharp-improve-code-quality/

## Tests
- xUnit 
- Moq 
- Fluent Assertions
- Bogus

# Web API Protection 
- Authorization overview: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-5.0
- Web API protection:https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-overview
 
