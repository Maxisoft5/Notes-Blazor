using Common.DTOs;
using Notes.Core.DTOs;

namespace Notes.Core.Interfaces
{
    public interface INotesService
    {
        public Task<PaginationResult<NoteDTO>> GetPaginated(Pagination<NoteDTO> pagination);
        public Task<NoteDTO> AddNew(NoteDTO noteDTO);
        public Task<NoteDTO> UpdateNote(NoteDTO noteDTO);
    }
}
