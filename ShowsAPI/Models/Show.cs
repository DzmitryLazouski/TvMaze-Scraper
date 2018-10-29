using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShowsAPI.Models
{
    public class Show
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Person> Cast { get; set; }
    }
}
