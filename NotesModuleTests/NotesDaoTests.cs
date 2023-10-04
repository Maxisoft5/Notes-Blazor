using DaoTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Notes.DataAccess.DAOs;
using Notes.Domain.Entities;
using System.Collections.Generic;
using System.Text;

namespace NotesModuleTests
{
    public class NotesDaoTests : BaseDaoTest
    {
        private readonly INotesDao _notesDao;

        public NotesDaoTests()
        {
            _notesDao = new NotesDao(DataContext, new Mock<ILogger<NotesDao>>().Object);
        }

        [Fact]
        public async void AddNoteTest_Should_Add()
        {
            // arrange
            var note = new Note() {
                AddedDateTime = DateTime.UtcNow,
                Body = "TestNote1",
                Title = "TestTitle1"
            };

            // act
            var added = await _notesDao.AddNote(note);

            // arrange
            Assert.True(added.Id > 0);
            Assert.Equal(note.AddedDateTime, added.AddedDateTime);
            Assert.Equal(note.Body, added.Body);
            Assert.Equal(note.Title, added.Title);
        }

        [Fact]
        public async Task AddNote_Without_Utc_Added_Time_Throws_UpdateDb_Exception()
        {
            // arrange
            var note = new Note()
            {
                AddedDateTime = DateTime.Now,
                Body = "TestNote1",
                Title = "TestTitle1"
            };

            // act
            // arrange
            await Assert.ThrowsAsync<DbUpdateException>( async () => 
                await _notesDao.AddNote(note));
        }

        [Fact]
        public async Task AddNote_With_MaxLengthBody()
        {
            // arrange
            const int maxBody = 5000;
            StringBuilder body = new StringBuilder();
            for (int i = 0; i < maxBody; i++)
            {
                body.Append("t");
            }
            var note = new Note()
            {
                AddedDateTime = DateTime.UtcNow,
                Body = body.ToString(),
                Title = "TestTitle1"
            };

            // act
            var added = await _notesDao.AddNote(note);

            Assert.Equal(maxBody ,added.Body.Length);

        }

        [Fact]
        public async void AddNote_WithHigherThanMaxLength_Throws_Exception()
        {
            // arrange
            const int maxBody = 5001;
            StringBuilder body = new StringBuilder();
            for (int i = 0; i < maxBody; i++)
            {
                body.Append("t");
            }
            var note = new Note()
            {
                AddedDateTime = DateTime.UtcNow,
                Body = body.ToString(),
                Title = "TestTitle1"
            };

            // act
            var exception = await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _notesDao.AddNote(note));
        }

        [Fact]
        public async void AddNote_Without_Title_Throws_Exception()
        {
            // arrange
            var note = new Note()
            {
                AddedDateTime = DateTime.UtcNow,
                Body = "body",
                Title = null
            };

            // act
            var exception = await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _notesDao.AddNote(note));
        }

        [Fact]
        public async void AddNote_Without_Body_Throws_Exception()
        {
            // arrange
            var note = new Note()
            {
                AddedDateTime = DateTime.UtcNow,
                Body = null,
                Title = "Title"
            };

            // act
            var exception = await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _notesDao.AddNote(note));
        }

        [Fact]
        public async void UpdateNoteTitle_Should_Be_OK()
        {
            // arrange
            var note = new Note()
            {
                AddedDateTime = DateTime.UtcNow.AddDays(1),
                Body = "Body",
                Title = "Title"
            };

            // act
            var added = await _notesDao.AddNote(note);

            added.Title = "Title2";

            var updated = await _notesDao.UpdateNote(added);

            Assert.True(added.Title == updated.Title);
            Assert.True(note.Body == updated.Body);
            Assert.True(note.AddedDateTime == updated.AddedDateTime);
        }

        [Fact]
        public async void Update_Body_Should_be_Ok()
        {
            // arrange
            var note = new Note()
            {
                AddedDateTime = DateTime.UtcNow,
                Body = "Body",
                Title = "Title"
            };

            // act
            var added = await _notesDao.AddNote(note);

            added.Body = "body2";

            var updated = await _notesDao.UpdateNote(added);

            Assert.True(added.Body == updated.Body);
            Assert.True(note.Title == updated.Title);
            Assert.True(note.AddedDateTime == updated.AddedDateTime);

        }

        [Fact]
        public async void Update_Body_Or_Title_With_Null_Throws_Exception()
        {
            // arrange
            var note = new Note()
            {
                AddedDateTime = DateTime.UtcNow,
                Body = "Body",
                Title = "Title"
            };

            // act
            var added = await _notesDao.AddNote(note);

            added.Body = null;

            var exceptionBody = await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _notesDao.UpdateNote(added));


            added.Body = "Body";
            added.Title = null;

            var exceptionTitle = await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _notesDao.UpdateNote(added));

        }

        [Fact]
        public async void GetPagination_Should_Return_With_Limit_Offset_OrderedByAddedTimeDesc()
        {
            // arrange
            for (int i = 0; i < 6; i++)
            {
                var note = new Note()
                {
                    AddedDateTime = DateTime.UtcNow,
                    Body = $"Body{i}",
                    Title = $"Title{i}"
                };

                var added = await _notesDao.AddNote(note);
            }
            const int limit = 5;
            const int offset = 0;

            // act
            var paginated = await _notesDao.GetPagination(new Common.DTOs.Pagination<Note>()
            {
                Offset = offset,
                Limit = limit,
            });

            // assert
            Assert.True(paginated.Result.Count() == limit);
            Note previous = null;
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

        }

        [Fact]
        public async void Pagination_LimitOffset_Test()
        {
            // arrange
            for (int i = 0; i < 10; i++)
            {
                var note = new Note()
                {
                    AddedDateTime = DateTime.UtcNow.AddDays(1),
                    Body = $"Body{i+1}",
                    Title = $"Title{i+1}"
                };

                var added = await _notesDao.AddNote(note);
            }
            const int limit = 5;
            const int offset = 5;

            // act
            var paginated = await _notesDao.GetPagination(new Common.DTOs.Pagination<Note>()
            {
                Offset = offset,
                Limit = limit,
            });

            // assert
            Assert.True(paginated.Result.Count() == limit);
            var list = paginated.Result.ToList();
            int index = 5;
            for (int i = 0; i != list.Count; i++)
            {
                var lastchar = list[i].Body[^1];
                int actualIndex = int.Parse(new char[] { lastchar });
                Assert.True(actualIndex == index - i);
            }
        }

        [Fact]
        public async void GetPaginated_With_Filters_Should_Return_Filtered_Result_Correct()
        {
            // arrange
            const int filterBodyCount = 3;
            const int filterTitleCount = 4;
            const int filterDaysAgoCount = 5;
            const string filterBody = "Filter_Body";
            const string filterTitle = "Filter_Body";
            DateTime dateAdded = DateTime.UtcNow;

            for (int i = 0; i < filterBodyCount; i++)
            {
                var note = new Note()
                {
                    AddedDateTime = DateTime.UtcNow,
                    Body = filterBody,
                    Title = $"test"
                };

                var added = await _notesDao.AddNote(note);
            }

            for (int i = 0; i < filterTitleCount; i++)
            {
                var note = new Note()
                {
                    AddedDateTime = DateTime.UtcNow,
                    Body = $"test",
                    Title = filterTitle
                };

                var added = await _notesDao.AddNote(note);
            }

            for (int i = 0; i < filterDaysAgoCount; i++)
            {
                var note = new Note()
                {
                    AddedDateTime = dateAdded,
                    Body = $"test",
                    Title = $"test"
                };

                var added = await _notesDao.AddNote(note);
            }

            // act
            var paginatedFilterBody = await _notesDao.GetPagination(new Common.DTOs.Pagination<Note>()
            {
                Offset = 0,
                Limit = filterBodyCount,
                BodyKeyWordFilter = filterBody
            });

            var paginatedFilterTitle = await _notesDao.GetPagination(new Common.DTOs.Pagination<Note>()
            {
                Offset = 0,
                Limit = filterTitleCount,
                TitleFilter = filterTitle
            });

            var paginatedFilterDate = await _notesDao.GetPagination(new Common.DTOs.Pagination<Note>()
            {
                Offset = 0,
                Limit = filterDaysAgoCount,
                CreatedAtFilter = dateAdded
            });

            // assert
            Assert.True(paginatedFilterBody.Result.Count() == filterBodyCount);
            Assert.True(paginatedFilterBody.Result.All(x => x.Body == filterBody));

            Assert.True(paginatedFilterTitle.Result.Count() == filterTitleCount);
            Assert.True(paginatedFilterTitle.Result.All(x => x.Title == filterTitle));

            Assert.True(paginatedFilterDate.Result.Count() == filterDaysAgoCount);
            Assert.True(paginatedFilterDate.Result.All(x => x.AddedDateTime.Year == dateAdded.Year
                && x.AddedDateTime.Month == dateAdded.Month && x.AddedDateTime.Day == dateAdded.Day));

        }
    }
}
