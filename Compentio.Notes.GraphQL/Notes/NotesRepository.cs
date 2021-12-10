namespace Compentio.Notes.GraphQL.Notes
{
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INotesRepository
    {
        Task<IEnumerable<NoteDao>> GetNotes();
        Task<NoteDao> GetNote(string noteId);
        Task<NoteDao> AddNote(NoteDao note);
        Task<NoteDao> UpdateNote(NoteDao note);
        Task DeleteNote(string noteId);
    }

    public class NotesRepository : INotesRepository
    {       
        private readonly IMongoCollection<NoteDao> _notes;

        public NotesRepository(MongoDbSettings options)
        {
            var mongoClient = new MongoClient(options.ConnectionString);           
            var database = mongoClient.GetDatabase(options.DatabaseName);
            _notes = database.GetCollection<NoteDao>("Notes");
        }

        public async Task<NoteDao> AddNote(NoteDao note)
        {
            await _notes.InsertOneAsync(note).ConfigureAwait(false);
            return note;
        }

        public async Task DeleteNote(string noteId)
        {
            await _notes.DeleteOneAsync(filter => filter.Id == noteId).ConfigureAwait(false);
        }

        public async Task<NoteDao> GetNote(string noteId)
        {
            return await _notes.Find(note => note.Id == noteId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<NoteDao>> GetNotes() => await _notes.Find(note => true).ToListAsync();

        public async Task<NoteDao> UpdateNote(NoteDao note)
        {
            await _notes.ReplaceOneAsync(n => n.Id == note.Id, note);
            return await Task.FromResult(note);
        }
    }
}
