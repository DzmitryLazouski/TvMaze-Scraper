using System.Linq;
using Scraper.Data.Entities;


namespace ShowsAPI.Persistence
{
    public interface IShowsRepository
    {
        IQueryable<Show> GetAll();
    }
}
