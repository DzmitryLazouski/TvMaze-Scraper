using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NLog;
using NUnit.Framework;
using ShowsAPI.Controllers;
using ShowsAPI.Logging;
using ShowsAPI.Models;
using ShowsAPI.Pagination;
using ShowsAPI.Persistence;

namespace ShowsAPI.Tests
{
    public class ShowTests
    {
        private const string ApiUrl = "http://show.azurewebsites.net/api/shows";
        private static readonly HttpClient Client = new HttpClient();
        private Logger _log;

        [OneTimeSetUp]
        public void ShowTestsSetUp()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        private List<Show> GetShows()
        {
            var shows = new List<Show>
            {
                new Show
                {
                    Id = 1,
                    Name = "Game of Thrones",
                    Cast = new List<Person>
                    {
                        new Person
                        {
                            Id = 9,
                            Name = "Dean Norris",
                            Birthday = "1963-04-08"
                        },
                        new Person
                        {
                            Id = 7,
                            Name = "Mike Vogel",
                            Birthday = "1979-07-17"
                        }
                    }

                },
                new Show
                {
                    Id = 4,
                    Name = "Big Bang Theory",
                    Cast = new List<Person>
                    {
                        new Person
                        {
                            Id = 6,
                            Name = "Michael Emerson",
                            Birthday = "1950-01-01"
                        }
                    }
                }
            };
            return shows;
        }

        [Test]
        [Explicit]
        public async Task GetReturnsActionResultsTest()
        {
            // Arrange
            var mockRepo = new Mock<IShowsRepository>();
            mockRepo.Setup(repo => repo.GetAll()).Returns(GetShows().AsQueryable());
            var controller = new ShowsController(mockRepo.Object, new LoggerManager());
            var paginationParameter = new PaginationParameterModel { PageNumber = 1, PageSize = 2 };

            // Act
            var result = await controller.Get(paginationParameter);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var model = okResult?.Value as List<Show>;
            Assert.AreEqual(2, model?.Count, $"Wrong number of shows: {model?.Count}");
        }

        [Test]
        public async Task ReturnShowsTest()
        {
            // Act
            var response = await Client.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            Assert.NotNull(responseString);
        }
    }
}