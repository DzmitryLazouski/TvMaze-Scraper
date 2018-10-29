using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Scraper.Contexts;
using Scraper.Models;
using Scraper.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Scraper
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient();
        public static int NotFoundCount { get; set; }

        static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Start");

            try
            {
                const int batchSize = 1000;
                int maxShowId;
                using (var context = new ShowsContext())
                {
                    maxShowId = context.Show.Max(s => s.Id);
                }

                for (var i = maxShowId + 1; NotFoundCount != 10; i += batchSize)
                {
                    var showList = await DownloadShows(i, i + batchSize, logger);
                    var showDataList = DeserializeShows(showList);
                    var castDataList = await DeserializeCasts(showDataList, logger);
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
                logger.LogError(ex.Message);
                logger.LogError(ex.StackTrace);
            }
        }

        private static async Task<List<string>> DownloadShows(int startShowId, int endShowId, ILogger logger)
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
                        logger.LogInformation(showUrl);

                        var result = await Client.GetAsync(showUrl);

                        while (result.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(500, 550)));
                            result = await Client.GetAsync(showUrl);
                        }

                        if (result.StatusCode == HttpStatusCode.NotFound)
                        {
                            NotFoundCount++;
                        }

                        var show = await result.Content.ReadAsStringAsync();

                        if (result.StatusCode != HttpStatusCode.NotFound )
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
                    logger.LogError(ex, $"Error while downloading show with Id = {i}.");
                }
            }

            logger.LogInformation($"{showList.Count} show(s) downloaded.");
            return showList;
        }

        private static List<ShowData> DeserializeShows(List<string> showsJson)
        {
            var showObjList = new List<ShowData>();

            foreach (var showJson in showsJson.ToList())
            {
                var showObj = JsonConvert.DeserializeObject<ShowData>(showJson);
                showObjList.Add(showObj);
            }

            return showObjList;
        }

        private static async Task<List<CastData>> DeserializeCasts(List<ShowData> showsData, ILogger logger)
        {
            var fullCastList = new List<CastData>();

            foreach (var show in showsData)
            {
                try
                {
                    var castJson = await DownloadCast(show.id);
                    var castDataList = JsonConvert.DeserializeObject<List<CastData>>(castJson);
                    foreach (var item in castDataList)
                    {
                        item.ShowId = show.id;
                    }
                    fullCastList.AddRange(castDataList);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, $"Error while downloading cast for show with Id = {show.id}");
                }
            }

            return fullCastList;
        }

        private static async Task<string> DownloadCast(string id)
        {
            var castUrl = $"http://api.tvmaze.com/shows/{id}/cast";

            Console.WriteLine(castUrl);

            var result = await Client.GetAsync(castUrl);

            if (result.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                result = await Client.GetAsync(castUrl);
            }

            var cast = await result.Content.ReadAsStringAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(500, 600)));
            return cast;
        }

        private static (List<Show> shows, List<Person> people) GetShowsInfo(List<ShowData> showDataList, List<CastData> castDataList)
        {
            var shows = new List<Show>();
            var peopleList = new List<Person>();
            foreach (var showData in showDataList)
            {
                if(string.IsNullOrEmpty(showData.id)) continue;

                var personDataList = castDataList.Where(x => x.ShowId == showData.id).Select(x => x.Person).ToList();
                var people = personDataList
                    .Select(x => new Person
                    {
                        Id = int.Parse(x.id),
                        Name = x.name,
                        Birthday = x.birthday,
                        ShowId = int.Parse(showData.id)
                    }).ToList();

                peopleList.AddRange(people);

                var show = new Show
                {
                    Id = int.Parse(showData.id),
                    Name = showData.name
                };
                shows.Add(show);
            }
            return (shows, peopleList);
        }

        private static async Task SaveShows(List<Show> shows)
        {
            using (var db = new ShowsContext())
            {
                await db.Show.AddRangeAsync(shows);
                await db.SaveChangesAsync();
            }
        }

        private static async Task SavePeople(List<Person> peopleList)
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