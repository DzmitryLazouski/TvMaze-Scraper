using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scraper.Data.Entities;
using Scraper.Data.Models;
using ShowsAPI.Persistence;

namespace ShowsAPI.Controllers
{
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IShowsRepository _showsRepository;
        private readonly IMapper _mapper;

        public ShowsController(IShowsRepository showsRepository, ILogger<ShowsController> logger, IMapper mapper)
        {
            _logger = logger;
            _showsRepository = showsRepository;
            _mapper = mapper;
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int pageSize = 5, [FromQuery]int pageNumber = 1)
        {
            var itemsOnPage = await _showsRepository.GetAll()
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            OrderCastByBirthdayDesc(itemsOnPage);

            _logger.LogInformation($"There are {itemsOnPage.Count} shows on page");

            return Ok(_mapper.Map<List<Show>, List<ShowModel>>(itemsOnPage));
        }

        private static void OrderCastByBirthdayDesc(IEnumerable<Show> showItems)
        {
            foreach (var showItem in showItems)
            {
                showItem.Cast = showItem.Cast.OrderByDescending(c => c.Birthday).ToList();
            }
        }
    }
}