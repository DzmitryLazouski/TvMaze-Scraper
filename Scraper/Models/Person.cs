using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Scraper.Models
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
