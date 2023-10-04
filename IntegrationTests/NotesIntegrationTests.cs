using IntegrationTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Notes.Core.DTOs;
using Notes.Core.Interfaces;
using Notes.Domain.Entities;
using System.Collections.Generic;

namespace IntegrationTests
{

    public class NotesIntegrationTests : BaseIntegrationTest
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly INotesService _notesService;

        public NotesIntegrationTests()
        {
            _webApplicationFactory = GetTestApplication();
            using (var scope = _webApplicationFactory.Services.CreateAsyncScope())
            {
                var scopedServices = scope.ServiceProvider;
                _notesService = scopedServices.GetRequiredService<INotesService>();
            }
        }

        [Fact]
        public async void AddNoteTest()
        {
            // arrange
            var note = new NoteDTO()
            {
                AddedDateTime = DateTime.UtcNow,
                Body = "TestBody1",
                Title = "TestTitle1"
            };

            // act
            var addedNote = await _notesService.AddNew(note);

            // assert
            Assert.Equal(note.AddedDateTime.Year, addedNote.AddedDateTime.Year);
            Assert.Equal(note.AddedDateTime.Month, addedNote.AddedDateTime.Month);
            Assert.Equal(note.AddedDateTime.Day, addedNote.AddedDateTime.Day);
            Assert.Equal(note.Body, addedNote.Body);
            Assert.Equal(note.Title, addedNote.Title);
        }

        [Fact]
        public async void UpdateNoteTest()
        {
            // arrange
            var note = new NoteDTO()
            {
                AddedDateTime = DateTime.UtcNow,
                Body = "TestBody1",
                Title = "TestTitle1"
            };

            // act
            var addedNote = await _notesService.AddNew(note);
            addedNote.Title = "TitleUpdate";
            addedNote.Body = "BodyUpdate";

            var updated = await _notesService.UpdateNote(addedNote);


            // assert
            Assert.Equal(addedNote.Title, updated.Title);
            Assert.Equal(addedNote.Body, updated.Body);
        }

        [Fact]
        public async void GetPaginatedFullTextSearchBodyTest()
        {
            // arrange
            const int limit = 5;
            string fullTextSearchWord = "Body";
            for (int i = 0; i < 10; i++)
            {
                var note = new NoteDTO()
                {
                    AddedDateTime = DateTime.UtcNow,
                    Body = $"{fullTextSearchWord} Test",
                    Title = $"Title{i}"
                };

                var added = await _notesService.AddNew(note);
            }

            var paginated = await _notesService.GetPaginated(new Common.DTOs.Pagination<NoteDTO>()
            {
                Limit = limit,
                Offset = 5,
                BodyKeyWordFilter = fullTextSearchWord
            });
            NoteDTO previous = null;
            foreach (var note in paginated.Result)
            {
                if (previous == null)
                {
                    previous = note;
                }
                else
                {
                    Assert.True(note.AddedDateTime < previous.AddedDateTime);
                }
            }
            Assert.Equal(limit, paginated.Result.Count());
            Assert.True(paginated.Result.All(x => x.Body.Contains(fullTextSearchWord)));
        }

        [Fact]
        public async void Pagination_Should_Keep_Filters()
        {
            // arrange
            const int limit = 5;
            string fullTextSearchWord = "Body";
            for (int i = 0; i < 10; i++)
            {
                var note = new NoteDTO()
                {
                    AddedDateTime = DateTime.UtcNow,
                    Body = $"{fullTextSearchWord} Test",
                    Title = $"Title{i}"
                };

                var added = await _notesService.AddNew(note);
            }

            for (int i = 0; i < 5; i++)
            {
                var note = new NoteDTO()
                {
                    AddedDateTime = DateTime.UtcNow,
                    Body = $"Test 123",
                    Title = $"Title{i}"
                };

                var added = await _notesService.AddNew(note);
            }

            // act
            var paginatedFirst = await _notesService.GetPaginated(new Common.DTOs.Pagination<NoteDTO>()
            {
                Limit = limit,
                Offset = 0,
                BodyKeyWordFilter = fullTextSearchWord
            });

            var paginatedSecond = await _notesService.GetPaginated(new Common.DTOs.Pagination<NoteDTO>()
            {
                Limit = limit,
                Offset = 5,
                BodyKeyWordFilter = fullTextSearchWord
            });

            NoteDTO previous = null;
            foreach (var note in paginatedFirst.Result)
            {
                if (previous == null)
                {
                    previous = note;
                }
                else
                {
                    Assert.True(note.AddedDateTime < previous.AddedDateTime);
                }
            }

            previous = null;
            foreach (var note in paginatedSecond.Result)
            {
                if (previous == null)
                {
                    previous = note;
                }
                else
                {
                    Assert.True(note.AddedDateTime < previous.AddedDateTime);
                }
            }

            // assert
            Assert.Equal(limit, paginatedFirst.Result.Count());
            Assert.True(paginatedFirst.Result.All(x => x.Body.Contains(fullTextSearchWord)));
            Assert.Equal(limit, paginatedSecond.Result.Count());
            Assert.True(paginatedSecond.Result.All(x => x.Body.Contains(fullTextSearchWord)));
        }

        [Fact]
        public async void PaginationDateFilterTest()
        {
            // arrange
            const int limit = 4;  
            var date = DateTime.UtcNow;
            for (int i = 0; i < limit; i++)
            {
                var note = new NoteDTO()
                {
                    AddedDateTime = date,
                    Body = $"Date Test",
                    Title = $"Title{i}"
                };

                var added = await _notesService.AddNew(note);
            }
            for (int i = 0; i < 6; i++)
            {
                var note = new NoteDTO()
                {
                    AddedDateTime = DateTime.UtcNow.AddDays(-2),
                    Body = $"Date Test",
                    Title = $"Title{i}"
                };

                var added = await _notesService.AddNew(note);
            }

            var paginatedFirst = await _notesService.GetPaginated(new Common.DTOs.Pagination<NoteDTO>()
            {
                Limit = limit,
                Offset = 0,
                CreatedAtFilter = date
            });

            Assert.Equal(limit, paginatedFirst.Result.Count());
            Assert.True(paginatedFirst.Result.All(x => x.AddedDateTime.Year == date.Year));
            Assert.True(paginatedFirst.Result.All(x => x.AddedDateTime.Month == date.Minute));
            Assert.True(paginatedFirst.Result.All(x => x.AddedDateTime.Day == date.Day));
        }
    }
}
