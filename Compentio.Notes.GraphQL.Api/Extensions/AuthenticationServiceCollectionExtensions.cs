namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Compentio.Notes.GraphQL.Api.Authentication;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public static class AuthenticationServiceCollectionExtensions
    {
        /// <summary>
        /// Add authentication bypass
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Notes";
                options.DefaultChallengeScheme = "Notes";
            }).AddScheme<AuthenticationSchemeOptions, NotesAuthenticationHandler>("Notes", null);

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy("DefaultPolicy", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                });
            });
            return services;
        }
    }
}
