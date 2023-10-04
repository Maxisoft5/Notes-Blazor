using Common.DTOs;
using Notes.Domain.Entities;

namespace Notes.DataAccess.DAOs
{
    public interface INotesDao
    {
        public Task<PaginationResult<Note>> GetPagination(Pagination<Note> paginatedNotes);
        public Task<Note> AddNote(Note note);
        public Task<Note> UpdateNote(Note note);
    }
}
