namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    public static class OpenApiCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Compentio.Notes.GraphQL.Api.xml");
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Compentio.Notes.GraphQL.Api", 
                    Version = "v1" 
                });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "oauth2",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "Bearer",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            //AuthorizationUrl = new Uri(azureAdOptions.AuthorizationUrl),
                            //TokenUrl = new Uri(azureAdOptions.TokenUrl),
                        }
                    }
                };

                c.AddSecurityDefinition("oauth2", securitySchema);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                            Scheme = "oauth2",
                            Name = "oauth2",
                            In = ParameterLocation.Header
                        },
                        new[] { "access_as_user" }
                    }
                });

                c.IncludeXmlComments(filePath);
            });

            return services;
        }

        public static void UseAppSwaggerUI(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Compentio.Notes.GraphQL.Api");
                //c.OAuthClientId(azureAdOptions.ClientId);
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                //c.OAuthScopes(azureAdOptions.Scopes);
                c.OAuthScopeSeparator(" ");
            });
        }
    }
}
