using Common.Infrastructure.Extensions;
using Infrastructure.EFCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DaoTests
{
    public abstract class BaseDaoTest
    {
        protected DataContext DataContext { get; set; }

        public BaseDaoTest()
        {
            IServiceCollection services = new ServiceCollection();
            var builder = new ConfigurationBuilder();
            string connection = "Host=localhost;Port=5432;Database=notesdb;Username=postgres;Password=051099";
            builder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
            {
                new KeyValuePair<string, string?>("ConnectionStrings:NotesConnection",  connection ),
            });
            var configuration = builder.Build();

            services.AddDatabaseConfiguration(configuration);
            var serviceProvider = services.BuildServiceProvider();
            DataContext = serviceProvider.GetRequiredService<DataContext>();
        }

    }
}
