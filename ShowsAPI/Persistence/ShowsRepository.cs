﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShowsAPI.Contexts;
using ShowsAPI.Models;

namespace ShowsAPI.Persistence
{
    public interface IShowsRepository
    {
        IQueryable<Show> GetAll();
        Task<Show> GetBy(int id);
    }

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
