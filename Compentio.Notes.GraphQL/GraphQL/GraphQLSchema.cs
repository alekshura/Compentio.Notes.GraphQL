using GraphQL.Types;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Compentio.Notes.GraphQL.GraphQL
{
    /// <summary>
    /// GraphQL Schema responsible for object queries and mutations
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GraphQLSchema : Schema
    {
        public GraphQLSchema(QueryGraphType query, MutationGraphType mutation, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = query;
            Mutation = mutation;
        }
    }
}
