using Microsoft.EntityFrameworkCore;
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
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder
                .UseLoggerFactory(MyConsoleLoggerFactory)
                .UseSqlServer(
                    "Server=tcp:showsserver.database.windows.net,1433;Initial Catalog=AllShows;Persist Security Info=False;User ID=Mitenka;Password=Sql11235!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
    }
}
