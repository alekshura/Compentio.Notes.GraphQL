namespace Compentio.Notes.GraphQL.Notes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public interface INotesService
    {
        Task<IEnumerable<Note>> GetNotes();
        Task<Note> GetNote(string noteId);
        Task<Note> AddNote(Note note);
        Task<Note> UpdateNote(Note note);
        Task DeleteNote(string noteId);
    }

    public class NotesService : INotesService
    {
        private readonly INotesMapper _mapper;
        private readonly INotesRepository _notesRepository;
        private readonly ILogger<NotesService> _logger;

        public NotesService(INotesRepository notesRepository, ILogger<NotesService> logger, INotesMapper mapper)
        {
            _notesRepository = notesRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Note> AddNote(Note note)
        {
            _logger.LogInformation($"Add note: '{note}' requested.");
            
            var noteDao =  await _notesRepository.AddNote(_mapper.MapToDao(note)).ConfigureAwait(false);
            
            return _mapper.MapFromDao(noteDao);
        }

        public async Task DeleteNote(string noteId)
        {
            _logger.LogInformation($"Delete note with noteId: '{noteId}' requested.");

            await _notesRepository.DeleteNote(noteId).ConfigureAwait(false);
        }

        public async Task<Note> GetNote(string noteId)
        {
            _logger.LogInformation($"Get note requested for noteId: '{noteId}'.");

            var noteDao =  await _notesRepository.GetNote(noteId).ConfigureAwait(false);

            return _mapper.MapFromDao(noteDao);
        }

        public async Task<IEnumerable<Note>> GetNotes()
        {
            _logger.LogInformation("Get notes requested.");

            var noteDaos = await _notesRepository.GetNotes().ConfigureAwait(false);

            return noteDaos.Select(dao => _mapper.MapFromDao(dao));
        }

        public async Task<Note> UpdateNote(Note note)
        {
            _logger.LogInformation($"Update note: '{note}' requested.");

            var noteDao = await _notesRepository.UpdateNote(_mapper.MapToDao(note)).ConfigureAwait(false);

            return _mapper.MapFromDao(noteDao);
        }
    }
}
