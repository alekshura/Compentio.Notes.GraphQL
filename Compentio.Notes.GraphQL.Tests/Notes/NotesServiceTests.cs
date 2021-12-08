namespace Compentio.Notes.GraphQL.Tests.Notes
{
    using System;
    using System.Threading.Tasks;
    using Xunit;
    using FluentAssertions;
    using Compentio.Notes.GraphQL.Notes;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Compentio.Notes.GraphQL.Tests.Notes.Mocks;

    public class NotesServiceTests
    {
        private readonly NotesRepositoryMock _notesRepositoryMock;
        private readonly Mock<ILogger<NotesService>> _loggerMock;

        private readonly NotesService _notesService;

        public NotesServiceTests()
        {
            _notesRepositoryMock = new();
            _loggerMock = new();
            _notesService = new NotesService(_notesRepositoryMock.Object, _loggerMock.Object, NotesMapper.Create());
        }


        [Fact]
        public async Task ShouldReturnListOfNotes()
        {
            // Arrange
            var mockedNotes = NotesMocks.NoteDaos;
            _notesRepositoryMock.MockGetNotes(mockedNotes);

            // Act            
            var notes = await _notesService.GetNotes().ConfigureAwait(false);

            // Assert
            notes.Should().BeEquivalentTo(mockedNotes);
        }
    }
}
