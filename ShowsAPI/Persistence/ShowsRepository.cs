using System.Collections.Generic;
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
    }
}
