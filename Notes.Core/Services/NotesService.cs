using AutoMapper;
using Common.DTOs;
using Notes.Core.DTOs;
using Notes.Core.Interfaces;
using Notes.DataAccess.DAOs;
using Notes.Domain.Entities;

namespace Notes.Core.Services
{
    public class NotesService : INotesService
    {
        private readonly INotesDao _notesDao;
        private readonly IMapper _mapper;

        public NotesService(INotesDao notesDao, IMapper mapper)
        {
            _notesDao = notesDao;
            _mapper = mapper;
        }

        public async Task<NoteDTO> AddNew(NoteDTO noteDTO)
        {
            var mappedNote = _mapper.Map<Note>(noteDTO);
            mappedNote.AddedDateTime = DateTime.UtcNow;
            var saved = await _notesDao.AddNote(mappedNote);
            return _mapper.Map<NoteDTO>(saved);
        }

        public async Task<PaginationResult<NoteDTO>> GetPaginated(Pagination<NoteDTO> pagination)
        {
            var mapped = _mapper.Map<Pagination<Note>>(pagination);
            var result = await _notesDao.GetPagination(mapped);
            return new PaginationResult<NoteDTO>()
            {
                Result = _mapper.Map<IEnumerable<NoteDTO>>(result.Result),
                TotalCount = result.TotalCount
            };
        }

        public async Task<NoteDTO> UpdateNote(NoteDTO noteDTO)
        {
            var mapped = _mapper.Map<Note>(noteDTO);
            var updated = await _notesDao.UpdateNote(mapped);
            return _mapper.Map<NoteDTO>(updated);
        }
    }
}
