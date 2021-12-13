using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Compentio.Notes.GraphQL.GraphQL
{
    [ExcludeFromCodeCoverage]
    public class GraphQLRequest
    {
        public string Query { get; set; }
        public JsonElement Variables { get; set; }
        public string OperationName { get; set; }
    }
}
