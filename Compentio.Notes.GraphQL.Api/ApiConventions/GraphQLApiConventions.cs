using Compentio.Notes.GraphQL.GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compentio.Notes.GraphQL.Api.ApiConventions
{
    public static class GraphQLApiConventions
    {
        [ProducesDefaultResponseType(typeof(GraphQLResponse))]
        [ProducesResponseType(typeof(GraphQLResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GraphQLResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public static void Post()
        { }

    }
}
