using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Scraper.Data.Entities;

namespace Scraper.Contexts
{
    public sealed class ShowsContext : DbContext
    {
        public ShowsContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<Show> Show { get; set; }
        public DbSet<Person> Actor { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder
                .UseSqlServer(configuration.GetConnectionString("ShowsDatabase"));
        }
    }
}
