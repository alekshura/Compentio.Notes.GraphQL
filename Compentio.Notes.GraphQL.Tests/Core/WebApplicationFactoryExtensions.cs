namespace Compentio.Notes.GraphQL.Tests.Core
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class WebApplicationFactoryExtensions
    {
        public static WebApplicationFactory<T> WithAuthentication<T>(this WebApplicationFactory<T> factory) where T : class
        {
            return AddAuthentication<T, TestAuthenticationHandler>(factory);
        }

        public static WebApplicationFactory<T> WithAuthenticationWithoutClaims<T>(this WebApplicationFactory<T> factory) where T : class
        {
            return AddAuthentication<T, TestAuthenticationHandlerWithoutClaims>(factory);
        }

        private static WebApplicationFactory<T> AddAuthentication<T, TAuthHandler>(this WebApplicationFactory<T> factory) 
            where T : class
            where TAuthHandler : TestAuthenticationHandlerBase
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TAuthHandler>("Test", options => { });
                });
            });
        }

        public static WebApplicationFactory<TFactory> WithService<TFactory, TService>(this WebApplicationFactory<TFactory> factory, Func<IServiceProvider, TService> implementationFactory) 
            where TService : class 
            where TFactory : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTransient(implementationFactory);
                });
            });
        }

        public static HttpClient CreateAndConfigureClient<T>(this WebApplicationFactory<T> factory) where T : class
        {
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            return client;
        }

    }
}
