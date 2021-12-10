using GraphQL.Types;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Compentio.Notes.GraphQL.GraphQL
{
    /// <summary>
    /// Schema responsible for object queries and mutations
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataSchema : Schema
    {
        public DataSchema(QueryGraphType query, MutationGraphType mutation, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = query;
            Mutation = mutation;
        }
    }
}
