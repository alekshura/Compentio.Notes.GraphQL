namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    public static class ConfigurationsCollectionExtensions
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Add configuration here

            services.Configure<GraphQL.MongoDbSettings>(configuration.GetSection(nameof(GraphQL.MongoDbSettings)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<GraphQL.MongoDbSettings>>().Value);
        }
    }
}
