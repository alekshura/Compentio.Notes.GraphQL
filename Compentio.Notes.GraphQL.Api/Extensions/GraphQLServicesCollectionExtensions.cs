using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Compentio.Notes.GraphQL.GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL;
using Compentio.Notes.GraphQL.Notes;
using GraphQL.SystemTextJson;
using GraphQL.DataLoader;
using GraphQL.Server.Authorization.AspNetCore;

namespace Compentio.Notes.GraphQL.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class GraphQLServicesCollectionExtensions
    {
        public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
        {
            services.AddTransient<IGraphQLProcessor, GraphQLProcessor>();               

            global::GraphQL.MicrosoftDI.GraphQLBuilderExtensions
                .AddGraphQL(services)
                .AddDocumentExecuter<DocumentExecuter>()
                .AddDocumentWriter<DocumentWriter>()
                .AddValidationRule<AuthorizationValidationRule>()
                .AddDataLoader()
                .AddSelfActivatingSchema<GraphQLSchema>()
                .AddSystemTextJson(options => options.PropertyNameCaseInsensitive = true);

            return services;
        }
    }
}
