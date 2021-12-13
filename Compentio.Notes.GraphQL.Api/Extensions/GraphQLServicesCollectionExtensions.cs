﻿using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
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
            services.AddTransient<IGraphQLProcessor, GraphQLProcessor>()
                .AddGraphQL().AddSelfActivatingSchema<DataSchema>()
                .AddGraphTypes()
                .AddSystemTextJson(options => options.PropertyNameCaseInsensitive = true);

            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            return services;
        }
    }
}
