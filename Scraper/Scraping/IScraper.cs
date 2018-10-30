using System.Threading.Tasks;

namespace Scraper.Scraping
{
    public interface IScraper
    {
        Task ScrapeAsync();
    }

}
