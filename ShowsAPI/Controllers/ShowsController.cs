using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShowsAPI.Models;
using ShowsAPI.Pagination;
using ShowsAPI.Persistence;
using StackExchange.Redis;

namespace ShowsAPI.Controllers
{
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IShowsRepository _showsRepository;
        private readonly IDatabase _cache;

        public ShowsController(IShowsRepository showsRepository, ILogger<ShowsController> logger)
        {
            _logger = logger;
            _showsRepository = showsRepository;
            _cache = Program.Connection.GetDatabase();
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]PaginationParameterModel paginationParameterModel)
        {
            var shows = _showsRepository.GetAll();

            var currentPage = paginationParameterModel.PageNumber;
            var pageSize = paginationParameterModel.PageSize;

            var showItems = shows.Skip((currentPage - 1) * pageSize)
                             .Take(pageSize).ToList();

            if (showItems.Count == 0)
            {
                _logger.LogInformation("No shows were found");
                return NotFound();
            }

            var key = $"ShowsCount on {DateTime.Now.ToShortDateString()}";
            var showsCountString = await _cache.StringGetAsync(key);
            int showsCount;
            if (string.IsNullOrEmpty(showsCountString))
            {
                showsCount = await shows.CountAsync();
                _logger.LogInformation($"There are {showsCount} shows in db.");
                await _cache.StringSetAsync(key, showsCount);
            }
            else
            {
                showsCountString.TryParse(out showsCount);
                _logger.LogInformation($"Number of shows = {showsCount} from Redis.");
            }

            HttpContext?.Response.Headers.Add("Paging-Headers", 
                JsonConvert.SerializeObject(paginationParameterModel.GetPaginationMetadata(showsCount)));

            OrderCastByBirthdayDesc(showItems);

            return Ok(showItems);
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