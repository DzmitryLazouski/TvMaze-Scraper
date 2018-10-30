using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
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
                .AddSingleton<IScraper, Scraping.Scraper>()
                .AddAutoMapper()
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
        }
    }
}