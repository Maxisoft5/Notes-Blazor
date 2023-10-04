using Common.DTOs;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.DataAccess.DAOs.Base;
using Notes.Domain.Entities;

namespace Notes.DataAccess.DAOs
{
    public class NotesDao : BaseNoteDao, INotesDao
    {
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private ILogger<NotesDao> Logger;

        public NotesDao(DataContext dataContext, ILogger<NotesDao> logger) : base(dataContext)
        {
            Logger = logger;
        }

        public async Task<Note> AddNote(Note note)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                await DataContext.AddAsync(note);
                await DataContext.SaveChangesAsync();
            } 
            catch (OperationCanceledException ex)
            {
                Logger.LogError($"Adding note canceled, {ex.Message}");
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.LogError($"Adding note concurrent error, {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Logger.LogError($"Adding note update error, {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Adding note error, {ex.Message}");
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
            return note;
        }

        public async Task<PaginationResult<Note>> GetPagination(Pagination<Note> pagination)
        {
            await semaphoreSlim.WaitAsync();
            IQueryable<Note> query = DataContext.Notes.AsQueryable<Note>().AsNoTracking();
            ApplyFilters(pagination, ref query);
            int totalCount = await query.CountAsync();
            query = query.OrderByDescending(x => x.AddedDateTime);
            query = query.Skip(pagination.Offset).Take(pagination.Limit);
            var notes = await query.ToListAsync();
            semaphoreSlim.Release();

            return new PaginationResult<Note>()
            {
                Result = notes,
                TotalCount = totalCount
            };
        }

        public async Task<Note> UpdateNote(Note note)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var savedNote = await DataContext.Notes.FirstOrDefaultAsync(x => x.Id == note.Id);
                savedNote.Title = note.Title;
                savedNote.Body = note.Body;
                await DataContext.SaveChangesAsync();
            }
            catch (OperationCanceledException ex)
            {
                Logger.LogError($"Update note canceled, {ex.Message}");
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.LogError($"Update note concurrent error, {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Logger.LogError($"Update note update error, {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Update note error, {ex.Message}");
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
            return note;
        }


        private IQueryable<Note> ApplyFilters(FiltersRequest filtersRequest, ref IQueryable<Note> query)
        {
            if (!string.IsNullOrEmpty(filtersRequest.TitleFilter))
            {
                query = query.Where(x => x.Title.Trim().ToLower().Contains(filtersRequest.TitleFilter.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(filtersRequest.BodyKeyWordFilter))
            {
                query = query.Where(x => EF.Functions.ToTsVector("english", x.Body)
                      .Matches(filtersRequest.BodyKeyWordFilter));
            }
            if (filtersRequest.CreatedAtFilter.HasValue)
            {
                query = query.Where(x => x.AddedDateTime.Year == filtersRequest.CreatedAtFilter.Value.Year
                    && x.AddedDateTime.Month == filtersRequest.CreatedAtFilter.Value.Month
                    && x.AddedDateTime.Day == filtersRequest.CreatedAtFilter.Value.Day);
            }
            return query;
        }
    }
}
