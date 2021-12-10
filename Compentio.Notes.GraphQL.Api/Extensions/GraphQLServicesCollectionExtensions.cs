using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using GraphQL.Types;
using Compentio.Notes.GraphQL.GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL;
using Compentio.Notes.GraphQL.Notes;
using GraphQL.SystemTextJson;

namespace Compentio.Notes.GraphQL.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class GraphQLServicesCollectionExtensions
    {
        public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
        {
            services.AddTransient<IGraphQLService, GraphQLService>()
                .AddGraphQL().AddSelfActivatingSchema<DataSchema>()
                .AddGraphTypes()
                .AddSystemTextJson();

            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            return services;
        }
    }
}
