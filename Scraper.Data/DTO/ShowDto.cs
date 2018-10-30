using System.Collections.Generic;

namespace Scraper.Data.DTO
{
    public class ShowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PersonDto> Cast { get; set; }
    }
}
