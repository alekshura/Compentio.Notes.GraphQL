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
		o.ValidationRules = DocumentValidator.CoreRules
			.Concat(new[] { new NoteValidationRule() })
			.Concat(new[] { new AuthorizationValidationRule(_authorizationService, _claimsPrincipalAccessor) });
		o.EnableMetrics = false;
		o.ThrowOnUnhandledException = true;
	});

	var response = JsonSerializer.Deserialize<GraphQLResponse>(result, 
	      new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
	return response;
}
```
Similar logic for schema execution you can implement in your own Middleware like in [this one](https://github.com/graphql-dotnet/examples/blob/master/src/AspNetCoreCustom/Example/GraphQLMiddleware.cs).

Next step, we should define our **GraphQL** schema. In my case it is used for data fetching and data mutations:

```cs
[ExcludeFromCodeCoverage]
public class GraphQLSchema : Schema
{
	public GraphQLSchema(QueryGraphType query, MutationGraphType mutation, IServiceProvider serviceProvider) 
	     : base(serviceProvider)
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
		Name = $"{GetType().Name}";
		Description = "Mutation for the entities in the service object graph.";
		this.AuthorizeWith("DefaultPolicy");

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
				notesService.DeleteNote(noteId);
				return $"The note with noteId: '{noteId}' has been successfully deleted from db.";
			}).AuthorizeWith("AdminPolicy");
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
		services.AddTransient<IGraphQLProcessor, GraphQLProcessor>();               

		global::GraphQL.MicrosoftDI.GraphQLBuilderExtensions
			.AddGraphQL(services)
			.AddDocumentExecuter<DocumentExecuter>()
			.AddDocumentWriter<DocumentWriter>()
			.AddDataLoader()
			.AddSelfActivatingSchema<GraphQLSchema>()
			.AddSystemTextJson(options => options.PropertyNameCaseInsensitive = true);

		return services;
	}
}
```
 - `AddGraphQL().AddSelfActivatingSchema<GraphQLSchema>()` registers our schema with all graph types
 - `AddDataLoader()` registers _[DataLoader](https://github.com/graphql/dataloader)_ for batch processing and caching _n + 1_ requests
 - `services.AddTransient<IGraphQLProcessor, GraphQLProcessor>()` reqisters our `GraphQLProcessor`
 
 ### Validation
In **GraphQL** [validation](https://graphql-dotnet.github.io/docs/getting-started/query-validation/) run when a query is executed. There is a predefined list of  _validation rules_ that are turned on by default. You can add your own _validation rules_ or clear out the existing ones by setting the `ValidationRules` property:

```cs
var result = await _schema.ExecuteAsync(_documentWriter, o =>
{
	o.Query = request.Query;
	o.Inputs = request.Variables.ToInputs();
	o.OperationName = request.OperationName;
	o.ValidationRules = DocumentValidator.CoreRules.Concat(new[] { new NoteValidationRule() });
	o.EnableMetrics = false;
	o.ThrowOnUnhandledException = true;
});
```
Let assume, that note's title should be less than 50 characters.  `ValidationRule` implementation can look like:

```cs
public class NoteValidationRule : IValidationRule
{
	public NoteValidationRule()
	{
	}

	public async Task<INodeVisitor> ValidateAsync(ValidationContext context)
	{
		return new NodeVisitors(
			new MatchingNodeVisitor<Argument>((arg, context) =>
			{
				ValidateAsync(arg, context, context.TypeInfo.GetArgument());
			})
		);
	}

	private void ValidateAsync(IHaveValue node, ValidationContext context, QueryArgument argument)
	{
		if (!IsNoteArgument(argument.Name))
			return;

		var note = context.Inputs.FirstOrDefault(x => IsNoteArgument(x.Key)).Value as Dictionary<string, object>;
		var noteTitle = note["title"] as string;

		if (!string.IsNullOrEmpty(noteTitle) && noteTitle.Length > 50)
		{
			context.ReportError(new ValidationError(context.Document.OriginalQuery, "1.0", $"Field 'title' in argument '{argument.Name}' can not be longer than 50", node));
		}
	}
	private static bool IsNoteArgument(string argumentName)
	{
		return argumentName.Equals("note", StringComparison.InvariantCultureIgnoreCase);
	}
}
```

### Authorization

While in example **GraphQL** endpoint defined in API controller, it uses defined authentication for it. 
[Authorization](https://graphql-dotnet.github.io/docs/getting-started/authorization/) in GraphQL based on Validation approach with using of `AuthorizationValidationRule`.
To add it to the project we need to do:
- Install [GraphQL.Server.Authorization.AspNetCore](https://www.nuget.org/packages/GraphQL.Server.Authorization.AspNetCore/) package
- register your policies, register `HttpContextAccessor` and 'IClaimsPrincipalAccessor':
```cs
services.AddAuthorization(o => {
                o.AddPolicy("DefaultPolicy", policyBuilder => policyBuilder.RequireAuthenticatedUser());
                o.AddPolicy("AdminPolicy", policyBuilder => policyBuilder.RequireClaim("role", "Admin"));
            });

            services.AddHttpContextAccessor().AddTransient<IClaimsPrincipalAccessor, DefaultClaimsPrincipalAccessor>();
```
- In `GraphQLProcessor` add `AuthorizationValidationRule`:
```cs
public async Task<GraphQLResponse> ProcessQuery(GraphQLRequest request)
{
	var result = await _schema.ExecuteAsync(_documentWriter, o =>
	{
		o.Query = request.Query;
		o.Inputs = request.Variables.ToInputs();
		o.OperationName = request.OperationName;
		o.ValidationRules = DocumentValidator.CoreRules
			.Concat(new[] { new NoteValidationRule() })
			.Concat(new[] { new AuthorizationValidationRule(_authorizationService, _claimsPrincipalAccessor) });
		o.EnableMetrics = false;
		o.ThrowOnUnhandledException = true;
	});

	var response = JsonSerializer.Deserialize<GraphQLResponse>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
	return response;
} 
```
Having it configured now we can add authorization for graph objects, for example only Admin has permissions to Note removal:

```cs
Field<StringGraphType>(
	"deleteNote",
	"Delete note from database.",
	new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "noteId" }),
	context =>
	{
		var noteId = context.GetArgument<string>("noteId");
		notesService.DeleteNote(noteId);
		return $"The note with noteId: '{noteId}' has been successfully deleted from db.";
	}).AuthorizeWith("AdminPolicy");

```

 
