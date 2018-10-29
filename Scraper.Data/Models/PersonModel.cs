using System.ComponentModel.DataAnnotations.Schema;

namespace Scraper.Data.Models
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }

        public int ShowId { get; set; }
        public ShowModel Show { get; set; }
    }
}
