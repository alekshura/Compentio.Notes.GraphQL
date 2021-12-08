namespace Compentio.Notes.GraphQL.Tests.Notes
{
    using Compentio.Notes.GraphQL.Tests.Core;
    using System.Threading.Tasks;
    using Xunit;
    using FluentAssertions;
    using Compentio.Notes.GraphQL.Notes;
    using Moq;
    using System.Net.Http.Json;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Compentio.Notes.GraphQL.Tests.Notes.Mocks;
    using System;
    using System.Linq;

    public class NotesApiIntegrationTests : IClassFixture<WebApplicationFactory<Api.Startup>>
    {
        private readonly WebApplicationFactory<Api.Startup> _factory;
        private readonly NotesRepositoryMock _notesRepositoryMock;
        private readonly NotesMapperMock _notesMapperMock;

        private const string notesBaseUrl = "api/notes";

        public NotesApiIntegrationTests(WebApplicationFactory<Api.Startup> factory)
        {
            _factory = factory;
            _notesRepositoryMock = new();
            _notesMapperMock = new();
        }


        [Fact]
        public async Task ShouldReturnListOfNotes()
        {
            // Arrange
            var httpClient = _factory.WithAuthentication()
                      .WithService(_ => _notesRepositoryMock.Object)
                      .WithService(_ => _notesMapperMock.Object)
                      .CreateAndConfigureClient();

            var mockedNoteDaos = NotesMocks.NoteDaos;
            var mockedNotes = NotesMocks.NoteDaos.Select(dao => NotesMapper.Create().MapFromDao(dao));
            foreach (var note in mockedNotes)
            {
                _notesMapperMock.MockMapFromDao(note);
            }

            _notesRepositoryMock.MockGetNotes(mockedNoteDaos);

            // Act
            var response = await httpClient.GetAsync(notesBaseUrl).ConfigureAwait(false);
            var notes = await response.Content.ReadFromJsonAsync<IEnumerable<Note>>().ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().ContainAll("application/json; charset=utf-8");          
        }

        [Fact]
        public async Task ShouldBeUnauthorized()
        {
            // Arrange
            var httpClient = _factory
                .WithService(_ => _notesRepositoryMock.Object)
                .WithService(_ => _notesMapperMock.Object)
                      .CreateAndConfigureClient();


            // Act
            var response = await httpClient.GetAsync(notesBaseUrl).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            _notesRepositoryMock.Verify(notes => notes.GetNotes(), Times.Never());
        }

        [Fact]
        public async Task ShouldBeForbidden()
        {
            // Arrange
            var httpClient = _factory.WithAuthenticationWithoutClaims()
                      .WithService(_ => _notesRepositoryMock.Object)
                      .WithService(_ => _notesMapperMock.Object)
                      .CreateAndConfigureClient();


            // Act
            Func<Task> getNotesTask = async () => { _ = await httpClient.GetAsync(notesBaseUrl).ConfigureAwait(false); };

            // Assert
            await getNotesTask.Should().ThrowAsync<System.Net.Http.HttpRequestException>();

            _notesRepositoryMock.Verify(notes => notes.GetNotes(), Times.Never());
        }
    }
}
