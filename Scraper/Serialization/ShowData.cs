using Newtonsoft.Json;

namespace Scraper.Serialization
{
    public class ShowData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
