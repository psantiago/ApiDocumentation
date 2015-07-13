using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExampleApi.Website.Models.v2
{
    public class App
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<AppUsers> Users { get; set; }

        public HashSet<int> RelatedApps { get; set; } 
    }
}