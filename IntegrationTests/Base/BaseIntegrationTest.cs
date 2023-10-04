using Common.Infrastructure.Extensions;
using Infrastructure.EFCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notes.Core;
using Notes.Core.Mapping.Extensions;

namespace IntegrationTests.Base
{
    public abstract class BaseIntegrationTest
    {
        protected WebApplicationFactory<Program> GetTestApplication()
        {
            var webFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.FirstOrDefault(d => d.ServiceType 
                        == typeof(DbContextOptions<DataContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    var uuId = Guid.NewGuid().ToString();
                    var builder = new ConfigurationBuilder();
                    string connection = "Host=localhost;Port=5432;Database=test-notesdb;Username=postgres;Password=051099";
                    builder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
                    {
                        new KeyValuePair<string, string?>("ConnectionStrings:NotesConnection",  connection ),
                    });
                    var configuration = builder.Build();
                    services.AddDatabaseConfiguration(configuration);
                    services.AddNotesCoreServices();
                    services.AddNotesAutoMapperProfiles();
                    var provider = services.BuildServiceProvider();
                    var context = provider.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                });
            });
            return webFactory;
        }

        protected void DeleteDatabase(WebApplicationFactory<Program> factory)
        {
            using (var scope = factory.Services.CreateAsyncScope())
            {
                var scopedServices = scope.ServiceProvider;
                var cxt = scopedServices.GetRequiredService<DataContext>();
                cxt.Database.EnsureDeleted();
            }
        }

    }
}
