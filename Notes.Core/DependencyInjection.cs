using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notes.Core.Interfaces;
using Notes.Core.Services;
using Notes.DataAccess.DAOs;

namespace Notes.Core
{
    public static class DependencyInjection
    {
        public static void AddNotesCoreServices(this IServiceCollection services)
        {
            services.AddScoped<INotesDao, NotesDao>();
            services.AddScoped<INotesService, NotesService>();
        }

    }
}
