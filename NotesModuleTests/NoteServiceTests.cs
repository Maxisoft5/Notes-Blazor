using AutoMapper;
using Common.DTOs;
using Moq;
using Notes.Core.DTOs;
using Notes.Core.Services;
using Notes.DataAccess.DAOs;
using Notes.Domain.Entities;

namespace NotesModuleTests
{
    public class NoteServiceTests
    {
        [Fact]
        public async void GetPaginated_ShouldReturn_TotalCount_And_Notes()
        {
            // arrange
            var mapper = new Mock<IMapper>();
            var noteRequest = new Pagination<NoteDTO>();
            var mappedNotes = new Pagination<Note>()
            {
                Limit = 5,
                Offset = 0,
                BodyKeyWordFilter = "test"
            };
            var result = new PaginationResult<Note>()
            {
                Result = new List<Note>() 
                {
                    new Note()
                    {
                        Id = 1,
                        Body = "note1"
                    }
                },
                TotalCount = 1
            };
            mapper.Setup(x => x.Map<Pagination<Note>>(noteRequest)).Returns(mappedNotes);
            mapper.Setup(x => x.Map<IEnumerable<NoteDTO>>(It.IsAny<IEnumerable<Note>>()))
                .Returns(new List<NoteDTO>()
                {
                    new NoteDTO() { Id = 1}
                });

            mapper.Setup(x => x.Map<PaginationResult<NoteDTO>>(result))
                .Returns(It.IsAny<PaginationResult<NoteDTO>>());

            var noteDao = new Mock<INotesDao>();
            noteDao.Setup(x => x.GetPagination(mappedNotes)).Returns(Task.FromResult(result));
            var noteService = new NotesService(noteDao.Object, mapper.Object);
            var actualResult = await noteService.GetPaginated(noteRequest);

            Assert.Equal(result.TotalCount, actualResult.TotalCount);
            Assert.Equal(result.Result.Count(), actualResult.Result.Count());
        }
    }
}
