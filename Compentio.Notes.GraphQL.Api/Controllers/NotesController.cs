namespace Compentio.Notes.GraphQL.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Compentio.Notes.GraphQL.Notes;
    using System.Collections.Generic;
    using System.Threading.Tasks;
        
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/notes")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        { 
            _notesService = notesService;
        }

        /// <summary>
        /// Returns list of notes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Note>> Get()
        {
            return await _notesService.GetNotes().ConfigureAwait(false);
        }

        /// <summary>
        /// Returns note by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<Note> GetNote(string id)
        {
            return await _notesService.GetNote(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Method adds new note
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost()]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<Note>> Add([FromBody] Note note)
        {
            return await _notesService.AddNote(note).ConfigureAwait(false);
        }

        /// <summary>
        /// Method updates the note
        /// </summary>
        /// <param name="id"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult<Note>> Update(string id, [FromBody] Note note)
        {
            var noteToUpdate = await _notesService.GetNote(id).ConfigureAwait(false);
            if (noteToUpdate == null)
            {
                return NotFound();
            }

            var updatedNote = await _notesService.UpdateNote(note).ConfigureAwait(false);
            return Accepted(updatedNote);
        }

        /// <summary>
        /// Method deletes the note
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> Delete(string id)
        {
            await _notesService.DeleteNote(id).ConfigureAwait(false);
            return Accepted();
        }
    }
}
