# Compentio.Notes.GraphQL

![GitHub top language](https://img.shields.io/github/languages/top/alekshura/Compentio.Notes.GraphQL)
![GitHub contributors](https://img.shields.io/github/contributors/alekshura/Compentio.Notes.GraphQL)

## Intro
- **GraphQL** is transport agnostic **query language** while **REST** is **_HTTP_ based architectural style** of designing _API_. 
Obviously, **GraphQL** typically served over _HTTP_, and it is  enough for it to expose one endpoint (mainly accepted _HTTP_ _POST_ requests, but it is not a must) since it is an "entrypoint" for **GraphQL** queries: it returns the data that's explicitly requested. 
- **REST** API, on the other hand, expose a suite of URLs each of which expose a single resource using various HTTP verbs (e.g. _GET_, _POST_, _PUT_, _PATCH_, or _DELETE_) with predefined Request and Response _DTOs_ that are controlled by _Backend_ in compliance with REST API design standards, for example [you can find them here](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design).

The repository contains Web API solution for **GraphQL** and **REST** API that execute the same CRUD operations.

## Run the App 
Application uses **MongoDB** database underneath, therefore **MongoDB** should be isntalled.
To do it run 

```sh
docker-compose -d up
```
in the solution directory. Additionally `Notes` collection should be added to **MongoDB**.


## Adding **GraphQL** to existing Web API

For **.NET** Web App you can add **GraphQL** to you app in a few ways: for example via separate endpoint in your controller, adding your custom **GraphQL** [Middleware](https://github.com/graphql-dotnet/examples/blob/master/src/AspNetCoreCustom/Example/GraphQLMiddleware.cs) or you can use _NUGET_ package [GraphQL.Server.Transports.AspNetCore](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore) and add [GraphQLHttpMiddleware](https://github.com/graphql-dotnet/server/blob/master/src/Transports.AspNetCore/GraphQLHttpMiddleware.cs) or even [GraphQLWebSocketsMiddleware](https://github.com/graphql-dotnet/server/blob/master/src/Transports.Subscriptions.WebSockets/GraphQLWebSocketsMiddleware.cs).

In the repository **GraphQL** has been added via controller:

```cs
[ApiController]
[Route("api")]
public class GraphQLController : ControllerBase
{
	private readonly IGraphQLProcessor _graphQLService;

	public GraphQLController(IGraphQLProcessor graphQLService)
	{
		_graphQLService = graphQLService;
	}

	[HttpPost("/graphql")]
	[ApiConventionMethod(typeof(GraphQLApiConventions), nameof(GraphQLApiConventions.Post))]
	public async Task<IActionResult> Post([FromBody] GraphQLRequest request)
	{
		var result = await _graphQLService.ProcessQuery(request);
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

```
`IGraphQLProcessor` service here is a service responsible for schema execution, exposes one method for processing GraphQL requests: `Task<GraphQLResponse> ProcessQuery(GraphQLRequest request)`:

```cs
public async Task<GraphQLResponse> ProcessQuery(GraphQLRequest request)
{
	var result = await _schema.ExecuteAsync(_documentWriter, o =>
	{
		o.Query = request.Query;
		o.Inputs = request.Variables.ToInputs();
		o.OperationName = request.OperationName;
		o.ValidationRules = DocumentValidator.CoreRules;
		o.EnableMetrics = false;
		o.ThrowOnUnhandledException = true;
	});

	var response = JsonSerializer.Deserialize<GraphQLResponse>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
	return response;
}
```
Similar logic for schema execution you can implement in your own Middleware like in [this one](https://github.com/graphql-dotnet/examples/blob/master/src/AspNetCoreCustom/Example/GraphQLMiddleware.cs).

Next step, we should define our **GraphQL** schema. In my case it is used for data fetching and data mutations:

```cs
[ExcludeFromCodeCoverage]
public class GraphQLSchema : Schema
{
	public GraphQLSchema(QueryGraphType query, MutationGraphType mutation, IServiceProvider serviceProvider) : base(serviceProvider)
	{
		Query = query;
		Mutation = mutation;
	}
}
```
`QueryGraphType` is a schema definition part responsible for fetching data. In this example `INotesService` service is used as data source for CRUD operation in `QueryGraphType` and `MutationGraphType` on the notes and it is injected into `QueryGraphType` and `MutationGraphType`:

```cs
[ExcludeFromCodeCoverage]
public class QueryGraphType : ObjectGraphType
{
	public QueryGraphType(INotesService notesService)
	{
		Name = "Queries";

		FieldAsync<NoteGraphType, Note>("note",
			"Gets a note by its id.",
			arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "noteId" }),
			resolve: context =>
			{
				var noteId = context.GetArgument<string>("noteId");
notesService.GetNote(noteId);
			});

		FieldAsync<ListGraphType<NoteGraphType>, IEnumerable<Note>>("notes", resolve: context => notesService.GetNotes());
	}
}


[ExcludeFromCodeCoverage]
public class NoteGraphType : ObjectGraphType<Note>
{
	public NoteGraphType()
	{
		Name = nameof(Note);
		Field<StringGraphType>("Id");
		Field<LongGraphType>("DocumentId");
		Field<StringGraphType>("Title");
		Field<StringGraphType>("Description");
		Field<DateTimeGraphType>("Timestamp");
	}
}
```  
In the example ([GitHub Code is here]()) `INotesService` is business layer for data operation on `INotesRepository` with **MongoDb** database. Obviously, that in real cloud native scenarios it can use any data source: REST, GraphQL, Database, etc.  

Similar, `MutationGraphType` defined for notes adding, update and delete as:

```cs
[ExcludeFromCodeCoverage]
public class MutationGraphType : ObjectGraphType
{
	public MutationGraphType(INotesService notesService)
	{
		Name = "Mutation";
		Description = "Mutation for the entities in the service object graph.";

		FieldAsync<NoteGraphType, Note>(
			"addNote",
			"Add note to database.",
			new QueryArguments(new QueryArgument<NonNullGraphType<NoteInputGraphType>>{ Name = "note" }),
			context =>
			{
				var note = context.GetArgument<Note>("note");
				return notesService.AddNote(note);
			});

		FieldAsync<NoteGraphType, Note>(
			"updateNote",
			"Update note in database.",
			new QueryArguments(
				new QueryArgument<NonNullGraphType<NoteInputGraphType>> { Name = "note" },
				new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "noteId" }),
			context =>
			{
				var note = context.GetArgument<Note>("note");
				note.Id = context.GetArgument<string>("noteId");
				return notesService.UpdateNote(note);
			});

		Field<StringGraphType>(
			"deleteNote",
			"Delete note from database.",
			new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "noteId" }),
			context =>
			{
				var noteId = context.GetArgument<string>("noteId");
				return notesService.DeleteNote(noteId);
			});
	}       
}

[ExcludeFromCodeCoverage]
public sealed class NoteInputGraphType : InputObjectGraphType<Note>
{
	public NoteInputGraphType()
	{
		Name = "NoteInput";
		Field(r => r.Description);
		Field(r => r.Title);
		Field(r => r.DocumentId);
		Field(r => r.Timestamp);
	}
}
```
In fact, that all graph types, `DocumentWriter`, `DocumentExecuter` should be registered in Web App dependency injection container. I prefer to define `GraphQLServicesCollectionExtensions` class to have separate configuration for **GraphQL** stuff:

```cs
[ExcludeFromCodeCoverage]
public static class GraphQLServicesCollectionExtensions
{
	public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
	{
		services.AddTransient<IGraphQLProcessor, GraphQLProcessor>()
			.AddGraphQL().AddSelfActivatingSchema<GraphQLSchema>()
			.AddGraphTypes()
			.AddSystemTextJson(options => options.PropertyNameCaseInsensitive = true);

		services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
		services.AddSingleton<DataLoaderDocumentListener>();
		services.AddSingleton<IDocumentWriter, DocumentWriter>();
		services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
		return services;
	}
}
```
`AddGraphTypes()` here scans the calling assembly for classes that implement `GraphQL.Types.IGraphType` and registers them as transients within the dependency injection, `AddGraphQL().AddSelfActivatingSchema<GraphQLSchema>()` - registers our schema.

 
