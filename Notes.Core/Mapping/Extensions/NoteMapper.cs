using Microsoft.Extensions.DependencyInjection;

namespace Notes.Core.Mapping.Extensions
{
    public static class NoteMapper
    {
        public static void AddNotesAutoMapperProfiles(this IServiceCollection services)
        {
             services.AddAutoMapper(typeof(NoteProfile));
        }
    }
}
