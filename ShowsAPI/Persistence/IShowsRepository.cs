using System.Linq;
using System.Threading.Tasks;
using Scraper.Data.Entities;


namespace ShowsAPI.Persistence
{
    public interface IShowsRepository
    {
        IQueryable<Show> GetAll();
        Task<Show> GetBy(int id);
    }
}
