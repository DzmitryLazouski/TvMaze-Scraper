using Newtonsoft.Json;

namespace Scraper.Serialization
{
    public class CastData
    {
        [JsonIgnore]
        public string ShowId { get; set; }
        public PersonData Person { get; set; }
    }
}
