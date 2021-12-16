using System.Threading.Tasks;
using Compentio.Notes.GraphQL.GraphQL;
using GraphQL.Types;
using GraphQL;
using GraphQL.SystemTextJson;
using GraphQL.Validation;
using System.Text.Json;
using System.Linq;
using Compentio.Notes.GraphQL.GraphQL.Validation;
using GraphQL.Server.Authorization.AspNetCore;

namespace Compentio.Notes.GraphQL.Notes
{
    public interface IGraphQLProcessor
    {
        Task<GraphQLResponse> ProcessQuery(GraphQLRequest request);
    }

    public class GraphQLProcessor : IGraphQLProcessor
    {
        private readonly ISchema _schema;
        private readonly IDocumentWriter _documentWriter;
        private readonly AuthorizationValidationRule _authorizationRule;

        public GraphQLProcessor(IDocumentWriter documentWriter, ISchema schema,  AuthorizationValidationRule authorizationRule)
        {
            _documentWriter = documentWriter;
            _schema = schema;
            _authorizationRule = authorizationRule;
        }
        

        public async Task<GraphQLResponse> ProcessQuery(GraphQLRequest request)
        {
            var result = await _schema.ExecuteAsync(_documentWriter, o =>
            {
                o.Query = request.Query;
                o.Inputs = request.Variables.ToInputs();
                o.OperationName = request.OperationName;
                o.ValidationRules = DocumentValidator.CoreRules
                    .Concat(new[] { new NoteValidationRule() })
                    .Concat(new[] { _authorizationRule });
                o.EnableMetrics = false;
                o.ThrowOnUnhandledException = true;
            });

            var response = JsonSerializer.Deserialize<GraphQLResponse>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return response;
        }        
    }
}
