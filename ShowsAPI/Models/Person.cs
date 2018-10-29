using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShowsAPI.Models
{
    public class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
        [JsonIgnore]
        public int ShowId { get; set; }
        [JsonIgnore]
        public Show Show { get; set; }
    }
}
