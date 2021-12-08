namespace Compentio.Notes.GraphQL.Tests.Notes.Mocks
{
    using Moq;
    using Compentio.Notes.GraphQL.Notes;

    public class NotesMapperMock : Mock<INotesMapper>
    {
        public NotesMapperMock MockMapFromDao(Note note)
        {
            Setup(mapper => mapper.MapFromDao(It.IsAny<NoteDao>())).Returns(note);
            return this;
        }        
    }
}
