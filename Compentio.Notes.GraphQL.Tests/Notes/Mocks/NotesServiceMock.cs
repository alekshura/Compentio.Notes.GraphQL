namespace Compentio.Notes.GraphQL.Tests.Notes.Mocks
{
    using Moq;
    using Compentio.Notes.GraphQL.Notes;
    using System.Collections.Generic;
    using System;

    public class NotesServiceMock : Mock<INotesService>
    {
        public NotesServiceMock MockGetNotes(IEnumerable<Note> notes)
        {
            Setup(service => service.GetNotes()).ReturnsAsync(notes);
            return this;
        }

        public NotesServiceMock MockGetNote(string noteId, Note note)
        {
            Setup(service => service.GetNote(It.Is<string>(i => i == noteId))).ReturnsAsync(note);
            return this;
        }

        public NotesServiceMock MockAddNote(Note note)
        {
            note.Id = Guid.NewGuid().ToString();
            Setup(service => service.AddNote(note)).ReturnsAsync(note);
            return this;
        }

        public NotesServiceMock MockModifyNote(Note note)
        {
            Setup(service => service.GetNote(It.Is<string>(i => i == note.Id))).ReturnsAsync(note);
            return this;
        }
    }
}
