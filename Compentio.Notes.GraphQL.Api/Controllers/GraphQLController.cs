namespace Compentio.Notes.GraphQL.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Compentio.Notes.GraphQL.Notes;
    using System.Threading.Tasks;
    using Compentio.Notes.GraphQL.GraphQL;
    using Compentio.Notes.GraphQL.Api.ApiConventions;

    [Authorize]
    [ApiController]
    [Route("api")]
    public class GraphQLController : ControllerBase
    {
        private readonly IGraphQLProcessor _graphQLProcessor;

        public GraphQLController(IGraphQLProcessor graphQLProcessor)
        {
            _graphQLProcessor = graphQLProcessor;
        }

        [HttpPost("/graphql")]
        [ApiConventionMethod(typeof(GraphQLApiConventions), nameof(GraphQLApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] GraphQLRequest request)
        {
            var result = await _graphQLProcessor.ProcessQuery(request);
            
            if (result.HasError)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
