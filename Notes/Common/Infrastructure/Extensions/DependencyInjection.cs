using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Common.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services,
                IConfiguration configuration)
        {
            var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren().AsEnumerable();
            var notesConnection = connectionStrings.FirstOrDefault(x => x.Key == "NotesConnection");
            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(notesConnection.Value);
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }, optionsLifetime: ServiceLifetime.Singleton, contextLifetime: ServiceLifetime.Singleton);

            return services;
        }
    }
}
