using System.ComponentModel.DataAnnotations.Schema;
using Scraper.Data.Models;

namespace Scraper.Data.Entities
{
    public class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }

        public int ShowId { get; set; }
        public Show Show { get; set; }
    }
}
