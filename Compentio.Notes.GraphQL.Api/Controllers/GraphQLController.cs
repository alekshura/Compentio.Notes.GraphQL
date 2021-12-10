namespace Compentio.Notes.GraphQL.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Compentio.Notes.GraphQL.Notes;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Compentio.Notes.GraphQL.GraphQL;

   // [Authorize]
    [ApiController]
    [Route("api")]
    public class GraphQLController : ControllerBase
    {
        private readonly IGraphQLService _graphQLService;

        public GraphQLController(IGraphQLService graphQLService)
        {
            _graphQLService = graphQLService;
        }

        [HttpPost("/graphql")]
        [ProducesResponseType(typeof(GraphQLResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error[]), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] GraphQLRequest request)
        {
            var result = await _graphQLService.ProcessRequest(request);
            if (result.HasError)
            {
                return BadRequest(result);
            }

            if (result.Data is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
