namespace Compentio.Notes.GraphQL.GraphQL
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.Json.Serialization;

    [ExcludeFromCodeCoverage]
    public class GraphQLResponse
    {
        public object Data { get; set; }

        public Error[] Errors { get; set; }

        [JsonIgnore]
        public bool HasError => Errors != null && Errors.Any();
    }

    [ExcludeFromCodeCoverage]
    public class Error
    {
        public string Message { get; set; }
        public Location[] Locations { get; set; }
        public Extension Extensions { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Location
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Extension
    {
        public string Code { get; set; }
        public string Number { get; set; }
        public string[] Codes { get; set; }
    }
}
