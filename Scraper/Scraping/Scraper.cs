using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scraper.Contexts;
using Scraper.Data.DTO;
using Scraper.Data.Entities;

namespace Scraper.Scraping
{
    public class Scraper : IScraper
    {
        private int _notFoundErrorrsCount;

        private readonly ILogger<Scraper> _logger;
        private readonly ShowsContext _context;
        private readonly HttpClient _client;
        private readonly ScraperSettings _scraperSettings;

        public Scraper(ILoggerFactory loggerFactory, ShowsContext context, IHttpClientFactory clientFactory, IScraperSettings scraperSettings)
        {
            _logger = loggerFactory.CreateLogger<Scraper>();
            _context = context;
            _client = clientFactory.CreateClient("Scraper");
            _scraperSettings = scraperSettings.GetScraperSettings();
        }

        public async Task ScrapeAsync()
        {
            try
            {
                var maxShowId = _context.Show.Max(s => s.Id);

                while(_notFoundErrorrsCount < _scraperSettings.MaxNotFoundErrorsBeforeBreak)
                {
                    var currentShowId = maxShowId + 1;
                    var showList = await DownloadShowsAsync(currentShowId, currentShowId + _scraperSettings.BatchSize);
                    var showDataList = DeserializeShows(showList);
                    var castDataList = await DeserializeCastsAsync(showDataList);
                    var (shows, people) = GetShowsInfo(showDataList, castDataList);

                    if (shows.Count > 0)
                    {
                        await SaveShows(shows);
                    }

                    if (people.Count > 0)
                    {
                        await SavePeople(people);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        private async Task<List<string>> DownloadShowsAsync(int startShowId, int endShowId)
        {
            _logger.LogInformation("Start Downloading Shows");
            _notFoundErrorrsCount = 0;
            var showList = new List<string>();
            var rnd = new Random();
            for (var i = startShowId; i < endShowId; i++)
            {
                try
                {
                    var showId = i;
                    await Task.Factory.StartNew(async () =>
                    {
                        var showUrl = $"{_scraperSettings.ShowUrl}{showId}";
                        _logger.LogInformation(showUrl);

                        var result = await _client.GetAsync(showUrl);

                        while (result.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(
                                rnd.Next(_scraperSettings.MinTimeDelay, _scraperSettings.MaxTimeDelay)));
                            result = await _client.GetAsync(showUrl);
                        }

                        if (result.StatusCode == HttpStatusCode.NotFound)
                        {
                            _notFoundErrorrsCount++;
                        }

                        var show = await result.Content.ReadAsStringAsync();

                        if (result.StatusCode != HttpStatusCode.NotFound)
                        {
                            showList.Add(show);
                        }
                    });

                    if (_notFoundErrorrsCount == _scraperSettings.MaxNotFoundErrorsBeforeBreak)
                        break;

                    await Task.Delay(TimeSpan.FromMilliseconds(
                        rnd.Next(_scraperSettings.MinTimeDelay, _scraperSettings.MaxTimeDelay)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while downloading show with Id = {i}.");
                }
            }

            _logger.LogInformation($"{showList.Count} show(s) downloaded.");
            return showList;
        }

        //TODO get rid of JsonConvert
        private List<ShowDto> DeserializeShows(IEnumerable<string> showsJson)
        {
            var showObjList = new List<ShowDto>();

            foreach (var showJson in showsJson.ToList())
            {
                var showObj = JsonConvert.DeserializeObject<ShowDto>(showJson);
                showObjList.Add(showObj);
            }

            return showObjList;
        }

        private async Task<List<CastDto>> DeserializeCastsAsync(IEnumerable<ShowDto> showsData)
        {
            var fullCastList = new List<CastDto>();

            foreach (var show in showsData)
            {
                try
                {
                    var castJson = await DownloadCastAsync(show.Id);
                    var castDataList = JsonConvert.DeserializeObject<List<CastDto>>(castJson);
                    foreach (var item in castDataList)
                    {
                        item.ShowId = show.Id;
                    }
                    fullCastList.AddRange(castDataList);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while downloading cast for show with Id = {show.Id}");
                }
            }

            return fullCastList;
        }

        private async Task<string> DownloadCastAsync(string id)
        {
            _logger.LogInformation("Start Downloading Cast");
            var castUrl = GetCastUrl(id);
            _logger.LogInformation(castUrl);

            var result = await _client.GetAsync(castUrl);

            if (result.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(_scraperSettings.MinTimeDelay));
                result = await _client.GetAsync(castUrl);
            }

            var cast = await result.Content.ReadAsStringAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(
                new Random().Next(_scraperSettings.MinTimeDelay, _scraperSettings.MaxTimeDelay)));
            return cast;
        }

        internal string GetCastUrl(string id)
            => string.Format(_scraperSettings.CastUrl, id);

        private static (List<Show> shows, List<Person> people) GetShowsInfo(List<ShowDto> showDataList, List<CastDto> castDataList)
        {
            var shows = new List<Show>();
            var peopleList = new List<Person>();
            foreach (var showData in showDataList)
            {
                if (string.IsNullOrEmpty(showData.Id)) continue;

                var personDataList = castDataList.Where(x => x.ShowId == showData.Id).Select(x => x.Person).ToList();
                var people = personDataList
                    .Select(x => new Person
                    {
                        Id = int.Parse(x.Id),
                        Name = x.Name,
                        Birthday = x.Birthday,
                        ShowId = int.Parse(showData.Id)
                    }).ToList();

                peopleList.AddRange(people);

                var show = new Show
                {
                    Id = int.Parse(showData.Id),
                    Name = showData.Name
                };
                shows.Add(show);
            }
            return (shows, peopleList);
        }


        private async Task SaveShows(IEnumerable<Show> shows)
        {
            await _context.Show.AddRangeAsync(shows);
            await _context.SaveChangesAsync();
        }

        private async Task SavePeople(IEnumerable<Person> peopleList)
        {
            foreach (var item in peopleList)
            {
                await _context.Actor.AddAsync(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
