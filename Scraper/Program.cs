using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Scraping;

namespace Scraper
{
    class Program
    {
        static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IScraperSettings, ScraperSettings>()
                .AddSingleton<IScraper, Scraping.Scraper>()
                .AddHttpClient()
                .BuildServiceProvider();

            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Start");

            var scraper = serviceProvider.GetService<IScraper>();
            await scraper.Scrape();

            logger.LogDebug("Finish");
        }
    }
}