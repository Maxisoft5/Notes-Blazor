using Common.Infrastructure.Extensions;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;

namespace Common.Tests.Infrastructure
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void DataContext_Registration_In_Single_Instance_And_With_Connection()
        {
            // arrange
            IServiceCollection services = new ServiceCollection();
            var builder = new ConfigurationBuilder();
            string connection = "Host=localhost;Port=5432;Database=notesdb;Username=postgres;Password=051099";
            builder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
            {
                new KeyValuePair<string, string?>("ConnectionStrings:NotesConnection",  connection ),
            });
            var configuration = builder.Build();
   
            // act
            services.AddDatabaseConfiguration(configuration);

            // assert
            var context = services.FirstOrDefault(x => x.Lifetime == ServiceLifetime.Singleton 
                && x.ServiceType == typeof(DataContext));
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetService<DataContext>();
            var connectionFromService = service.Database.GetConnectionString();

            Assert.NotNull(context);
            Assert.Equal(connection, connectionFromService);
            Assert.Equal(ServiceLifetime.Singleton, context.Lifetime);
        }

    }
}
