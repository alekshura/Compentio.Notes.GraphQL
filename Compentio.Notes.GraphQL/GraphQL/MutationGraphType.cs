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
}
