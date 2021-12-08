namespace Compentio.Notes.GraphQL.Tests.Notes.Mocks
{
    using Moq;
    using Compentio.Notes.GraphQL.Notes;
    using System.Collections.Generic;
    using System;

    public class NotesRepositoryMock : Mock<INotesRepository>
    {
        public NotesRepositoryMock MockGetNotes(IEnumerable<NoteDao> notes)
        {
            Setup(service => service.GetNotes()).ReturnsAsync(notes);
            return this;
        }

        public NotesRepositoryMock MockGetNote(string noteId, NoteDao note)
        {
            Setup(service => service.GetNote(It.Is<string>(i => i == noteId))).ReturnsAsync(note);
            return this;
        }
        public NotesRepositoryMock MockAddNote(NoteDao note)
        {
            note.Id = Guid.NewGuid().ToString();
            Setup(service => service.AddNote(note)).ReturnsAsync(note);
            return this;
        }

        public NotesRepositoryMock MockModifyNote(NoteDao note)
        {
            Setup(service => service.GetNote(It.Is<string>(i => i == note.Id))).ReturnsAsync(note);
            return this;
        }
    }
}
