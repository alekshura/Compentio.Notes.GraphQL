namespace Compentio.Notes.GraphQL.GraphQL
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class GraphQLRequest
    {
        public string Query { get; set; }
        public IDictionary<string, object> Variables { get; set; }
    }
}
