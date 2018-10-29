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
            //Database.EnsureCreated();
        }
        public DbSet<Show> Show { get; set; }
        public DbSet<Person> Actor { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.EnableSensitiveDataLogging();
        //    optionsBuilder
        //        .UseLoggerFactory(MyConsoleLoggerFactory)
        //        //.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Shows;Trusted_Connection=True;");
        //        .UseSqlServer("Server=tcp:showsserver.database.windows.net,1433;Initial Catalog=Shows1000;Persist Security Info=False;User ID=Mitenka;Password=Sql11235!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        //}
    }
}
