using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ShowsAPI.Logging;
using ShowsAPI.Models;
using ShowsAPI.Pagination;
using ShowsAPI.Persistence;
using StackExchange.Redis;

namespace ShowsAPI.Controllers
{
    [Route("api/[controller]")]
    public class ShowsController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IShowsRepository _showsRepository;
        private IDatabase cache;

        public ShowsController(IShowsRepository showsRepository, ILoggerManager logger)
        {
            _logger = logger;
            _showsRepository = showsRepository;
            cache = Program.Connection.GetDatabase();
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
                _logger.LogInfo("No shows were found");
                return NotFound();
            }

            var key = $"ShowsCount on {DateTime.Now.ToShortDateString()}";
            var showsCountString = await cache.StringGetAsync(key).ConfigureAwait(false);
            int showsCount;
            if (string.IsNullOrEmpty(showsCountString))
            {
                showsCount = await shows.CountAsync();
                _logger.LogInfo($"There are {showsCount} shows in db.");
                await cache.StringSetAsync(key, showsCount).ConfigureAwait(false);
            }
            else
            {
                showsCountString.TryParse(out showsCount);
                _logger.LogInfo($"Number of shows = {showsCount} from Redis.");
            }

            HttpContext?.Response.Headers.Add("Paging-Headers",
                JsonConvert.SerializeObject(paginationParameterModel.GetPaginationMetadata(showsCount)));

            foreach (var showItem in showItems)
            {
                showItem.Cast = showItem.Cast.OrderByDescending(c => c.Birthday).ToList();
            }

            return Ok(showItems);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var key = $"MyEntity:{id}";

            var json = await cache.StringGetAsync(key).ConfigureAwait(false);
            var show = string.IsNullOrWhiteSpace(json)
                ? default(Show)
                : JsonConvert.DeserializeObject<Show>(json);

            if (show is null)
            {
                show = await _showsRepository.GetBy(id);
                if (show is null)
                {
                    _logger.LogInfo($"Show with id {id} was not found");
                    return NotFound();
                }
                show.Cast = show.Cast.OrderByDescending(c => c.Birthday).ToList();

                await cache.StringSetAsync(key, JsonConvert.SerializeObject(show)).ConfigureAwait(false);
                await cache.KeyExpireAsync(key, TimeSpan.FromMinutes(5)).ConfigureAwait(false);
            }

            return Ok(show);
        }
    }
}