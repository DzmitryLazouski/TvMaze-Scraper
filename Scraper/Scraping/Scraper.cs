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
        public static int NotFoundCount { get; set; }

        private readonly ILogger<Scraper> _logger;
        private readonly HttpClient _client;
        private readonly ScraperSettings _scraperSettings;

        public Scraper(ILoggerFactory loggerFactory, IHttpClientFactory clientFactory, IScraperSettings scraperSettings)
        {
            _logger = loggerFactory.CreateLogger<Scraper>();
            _client = clientFactory.CreateClient("Scraper");
            _scraperSettings = scraperSettings.GetScraperSettings();
        }

        public async Task Scrape()
        {
            try
            {
                int maxShowId;
                using (var context = new ShowsContext())
                {
                    maxShowId = context.Show.Max(s => s.Id);
                }

                while(NotFoundCount < _scraperSettings.MaxNotFoundErrorsBeforeBreak)
                {
                    var currentShowId = maxShowId + 1;
                    var showList = await DownloadShows(currentShowId, currentShowId + _scraperSettings.BatchSize);
                    var showDataList = DeserializeShows(showList);
                    var castDataList = await DeserializeCasts(showDataList);
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
                Console.WriteLine(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        private async Task<List<string>> DownloadShows(int startShowId, int endShowId)
        {
            NotFoundCount = 0;
            var showList = new List<string>();
            var rnd = new Random();
            for (var i = startShowId; i < endShowId; i++)
            {
                try
                {
                    var showId = i;
                    await Task.Factory.StartNew(async () =>
                    {
                        var showUrl = $"http://api.tvmaze.com/shows/{showId}";
                        Console.WriteLine(showUrl);
                        _logger.LogInformation(showUrl);

                        var result = await _client.GetAsync(showUrl);

                        while (result.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(500, 550)));
                            result = await _client.GetAsync(showUrl);
                        }

                        if (result.StatusCode == HttpStatusCode.NotFound)
                        {
                            NotFoundCount++;
                        }

                        var show = await result.Content.ReadAsStringAsync();

                        if (result.StatusCode != HttpStatusCode.NotFound)
                        {
                            showList.Add(show);
                        }
                    });

                    if (NotFoundCount == 10)
                        break;

                    await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(500, 550)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while downloading show with Id = {i}.");
                }
            }

            _logger.LogInformation($"{showList.Count} show(s) downloaded.");
            return showList;
        }

        private List<ShowDto> DeserializeShows(List<string> showsJson)
        {
            var showObjList = new List<ShowDto>();

            foreach (var showJson in showsJson.ToList())
            {
                var showObj = JsonConvert.DeserializeObject<ShowDto>(showJson);
                showObjList.Add(showObj);
            }

            return showObjList;
        }


        private async Task<List<CastDto>> DeserializeCasts(List<ShowDto> showsData)
        {
            var fullCastList = new List<CastDto>();

            foreach (var show in showsData)
            {
                try
                {
                    var castJson = await DownloadCast(show.Id);
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

        private async Task<string> DownloadCast(string id)
        {
            // TODO
            var castUrl = $"http://api.tvmaze.com/shows/{id}/cast";

            Console.WriteLine(castUrl);

            var result = await _client.GetAsync(castUrl);

            if (result.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                result = await _client.GetAsync(castUrl);
            }

            var cast = await result.Content.ReadAsStringAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(500, 600)));
            return cast;
        }

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
            using (var db = new ShowsContext())
            {
                await db.Show.AddRangeAsync(shows);
                await db.SaveChangesAsync();
            }
        }

        private async Task SavePeople(IEnumerable<Person> peopleList)
        {
            foreach (var item in peopleList)
            {
                using (var newContext = new ShowsContext())
                {
                    await newContext.Actor.AddAsync(item);
                    await newContext.SaveChangesAsync();
                }
            }
        }
    }
}
