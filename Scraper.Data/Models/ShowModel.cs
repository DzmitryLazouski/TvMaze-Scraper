using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scraper.Data.Models
{
    public class ShowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PersonModel> Cast { get; set; }
    }
}
