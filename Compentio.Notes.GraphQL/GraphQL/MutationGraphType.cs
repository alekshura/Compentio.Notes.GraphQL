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

        public sealed class NoteInputGraphType : InputObjectGraphType<Note>
        {
            public NoteInputGraphType()
            {
                Name = "NoteInput";
                Field(r => r.Description).Description("Description of the note");
                Field(r => r.Title).Description("Note title");
                Field(r => r.DocumentId).Description("Linked document id");
                Field(r => r.Timestamp).Description("Modification timestamp");
            }
        }
    }


    
}
