namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public static class ExceptionHandlerExtensions
    {
        public static void AddAppProblemDetails(this IServiceCollection services, IHostEnvironment env)
        {
            services.AddProblemDetails(setup =>
            {
                setup.IncludeExceptionDetails = (ctx, ex) => env.IsDevelopment();
            });
        }

        public static void UseAppExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseProblemDetails();
        }
    }
}
