using Compentio.Notes.GraphQL.Notes;
using GraphQL;
using GraphQL.Types;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Compentio.Notes.GraphQL.GraphQL
{
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
                    context.UserContext.SetFetchId(noteId);
                    return notesService.GetNote(noteId);
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
}
