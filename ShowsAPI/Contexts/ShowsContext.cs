using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ShowsAPI.Models;

namespace ShowsAPI.Contexts
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
        public ShowsContext(DbContextOptions<ShowsContext> options) : base(options)
        {
        }
        public DbSet<Show> Show { get; set; }
        public DbSet<Person> Actor { get; set; }
    }
}
