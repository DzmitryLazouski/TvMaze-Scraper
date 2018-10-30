using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Scraper.Scraping
{
    public class ScraperSettings : IScraperSettings
    {
        public int BatchSize { get; set; }
        public int MaxNotFoundErrorsBeforeBreak { get; set; }
        public int MinTimeDelay { get; set; }
        public int MaxTimeDelay { get; set; }
        public string ShowUrl { get; set; }

        public ScraperSettings GetScraperSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var scraperSettings = new ScraperSettings
            {
                BatchSize = int.Parse(configuration["BatchSize"]),
                MaxNotFoundErrorsBeforeBreak = int.Parse(configuration["MaxNotFoundErrosBeforeBreak"]),
                MinTimeDelay = int.Parse(configuration["MinTimeDelay"]),
                MaxTimeDelay = int.Parse(configuration["MaxTimeDelay"]),
                ShowUrl = configuration["ShowUrl"]
            };
            return scraperSettings;
        }
    }
}
