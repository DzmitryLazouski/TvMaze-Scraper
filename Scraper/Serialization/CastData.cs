using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper
{
    public class CastData
    {
        [JsonIgnore]
        public string ShowId { get; set; }
        public PersonData Person { get; set; }
    }
}
