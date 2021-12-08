namespace Compentio.Notes.GraphQL.Tests.Notes.Mocks
{
    using Bogus;
    using Compentio.Notes.GraphQL.Notes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class NotesMocks
    {
        public static IEnumerable<Note> Notes 
        {
            get 
            {
                Randomizer.Seed = new Random(8675309);

                var titles = new[] { "Info", "Document", "Attention", "Warning" };
                var testNotes = new Faker<Note>()
                    .StrictMode(true)
                    .RuleFor(note => note.Id, f => f.Random.Guid().ToString())
                    .RuleFor(note => note.DocumentId, f => f.Random.Number(1, 100))
                    .RuleFor(note => note.Title, f => f.PickRandom(titles))
                    .RuleFor(note => note.Description, f => f.Lorem.Sentences())
                    .RuleFor(note => note.Timestamp, f => f.Date.Past());

                var notes = testNotes.Generate(10);
                return notes.AsEnumerable();
            }
        }

        public static IEnumerable<NoteDao> NoteDaos
        {
            get
            {
                Randomizer.Seed = new Random(8675309);

                var titles = new[] { "Info", "Document", "Attention", "Warning" };
                var testNotes = new Faker<NoteDao>()
                    .StrictMode(true)
                    .RuleFor(note => note.Id, f => f.Random.Guid().ToString())
                    .RuleFor(note => note.DocumentId, f => f.Random.Number(1, 100))
                    .RuleFor(note => note.Title, f => f.PickRandom(titles))
                    .RuleFor(note => note.Description, f => f.Lorem.Sentences())
                    .RuleFor(note => note.Timestamp, f => f.Date.Past());

                var notes = testNotes.Generate(10);
                return notes.AsEnumerable();
            }
        }
    }
}
