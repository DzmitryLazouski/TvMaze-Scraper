using Newtonsoft.Json;

namespace Scraper.Serialization
{
    public class PersonData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "birthday")]
        public string Birthday { get; set; }
    }
}