using AutoMapper;
using Common.DTOs;
using Notes.Core.DTOs;
using Notes.Domain.Entities;

namespace Notes.Core.Mapping
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<Note, NoteDTO>().ReverseMap();
            CreateMap<Pagination<Note>, Pagination<NoteDTO>>().ReverseMap();
        }
    }
}
