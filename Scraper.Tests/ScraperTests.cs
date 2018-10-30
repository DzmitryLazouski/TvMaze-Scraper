using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Scraper.Scraping;

namespace Scraper.Tests
{
    [TestFixture]
    public class ScraperTests
    {
        private readonly Scraping.Scraper _scraper;
        public ScraperTests()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IScraper, Scraping.Scraper>();

            var serviceProvider = services.BuildServiceProvider();

            _scraper = (Scraping.Scraper) serviceProvider.GetService<IScraper>();
        }

        [Test]
        public void ScrapeTest()
        {
            // TODO mock
        }
    }
}
