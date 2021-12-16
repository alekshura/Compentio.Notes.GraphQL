using Compentio.Notes.GraphQL.Api.Authentication;
using GraphQL.Server.Authorization.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Compentio.Notes.GraphQL.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationServiceCollectionExtensions
    {
        /// <summary>
        /// Add authentication for REST cintrollers and GraphQL endpoint
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            const string defaultAuthenticateScheme = "Notes";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = defaultAuthenticateScheme;
                options.DefaultChallengeScheme = defaultAuthenticateScheme;
            }).AddScheme<AuthenticationSchemeOptions, AdminAuthenticationHandler>(defaultAuthenticateScheme, null);


            // Use UserAuthenticationHandler scheme. In this case used does not have permissions to note deletion
            //.AddScheme<AuthenticationSchemeOptions, UserAuthenticationHandler>(defaultAuthenticateScheme, null);

            services.AddAuthorization(o => {
                o.AddPolicy("DefaultPolicy", policyBuilder => policyBuilder.RequireAuthenticatedUser());
                o.AddPolicy("AdminPolicy", policyBuilder => policyBuilder.RequireClaim("role", "Admin"));
            });

            services.AddHttpContextAccessor().AddTransient<IClaimsPrincipalAccessor, DefaultClaimsPrincipalAccessor>();
            return services;
        }
    }
}
