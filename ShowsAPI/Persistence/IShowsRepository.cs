using System.Linq;
using System.Threading.Tasks;
using ShowsAPI.Models;


namespace ShowsAPI.Persistence
{
    public interface IShowsRepository
    {
        IQueryable<Show> GetAll();
        Task<Show> GetBy(int id);
    }
}
