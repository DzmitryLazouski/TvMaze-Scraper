using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scraper.Data.Entities;
using ShowsAPI.Contexts;

namespace ShowsAPI.Persistence
{
    public class ShowsRepository : IShowsRepository
    {
        private readonly ShowsContext _showsContext;
        public ShowsRepository(ShowsContext showsContext)
        {
            _showsContext = showsContext;
        }

        public IQueryable<Show> GetAll()
            => _showsContext.Show.Include(s => s.Cast);


        public async Task<Show> GetBy(int id)
            => await _showsContext.Show.Where(s => s.Id == id)
                .Include(x => x.Cast).FirstOrDefaultAsync();
    }
}
