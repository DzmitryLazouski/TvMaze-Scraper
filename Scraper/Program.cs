using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Contexts;
using Scraper.Scraping;

namespace Scraper
{
    class Program
    {
        static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddHttpClient()
                .AddDbContext<ShowsContext>(options => options.UseSqlServer(configuration.GetConnectionString("ShowsDatabase")))
                .AddSingleton<IScraperSettings, ScraperSettings>()
                .AddSingleton<IScraper, Scraping.Scraper>()
                .BuildServiceProvider();

            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var scraper = serviceProvider.GetService<IScraper>();
            await scraper.ScrapeAsync();
        }
    }
}