using Compentio.Notes.GraphQL.Notes;
using GraphQL;
using GraphQL.Types;
using System.Diagnostics.CodeAnalysis;

namespace Compentio.Notes.GraphQL.GraphQL
{
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
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<NoteInputGraphType>>
                    {
                        Name = "note",
                        Description = "User note"
                    }),
                context =>
                {
                    var note = context.GetArgument<Note>("note");
                    return notesService.AddNote(note);
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
}
