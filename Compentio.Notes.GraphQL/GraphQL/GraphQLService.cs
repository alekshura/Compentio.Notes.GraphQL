using System;
using System.Linq;
using System.Threading.Tasks;
using Compentio.Notes.GraphQL.GraphQL;
using GraphQL.Types;
using GraphQL;
using GraphQL.Validation;
using System.Text.Json;

namespace Compentio.Notes.GraphQL.Notes
{

    public interface IGraphQLService
    {
        Task<GraphQLResponse> ProcessRequest(GraphQLRequest request);
    }

    public class GraphQLService : IGraphQLService
    {
        private readonly ISchema _schema;
        private readonly IDocumentWriter _documentWriter;

        public GraphQLService(IDocumentWriter documentWriter, ISchema schema)
        {
            _documentWriter = documentWriter;
            _schema = schema;
        }
        

        public async Task<GraphQLResponse> ProcessRequest(GraphQLRequest request)
        {
            var result = await _schema.ExecuteAsync(_documentWriter, o =>
            {
                o.Query = request.Query;
                o.Inputs = new Inputs(request.Variables);
                o.ValidationRules = DocumentValidator.CoreRules;
                o.EnableMetrics = false;
            });

            var response = JsonSerializer.Deserialize<GraphQLResponse>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return response;
        }        
    }
}
