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
        public static readonly LoggerFactory MyConsoleLoggerFactory =
            new LoggerFactory(new[]
            {
                new ConsoleLoggerProvider(
                    (category, level) =>
                        category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information, true)
            });
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
                .UseLoggerFactory(MyConsoleLoggerFactory)
                .UseSqlServer(configuration.GetConnectionString("ShowsDatabase"));
        }
    }
}
