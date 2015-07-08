using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExampleApi.Website.Models.v2
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CountyEnum
    {
        Unknown = 0,
        Monongalia,
        Marion
    }

    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public Guid? SessionGuid { get; set; }

        [Required]
        public CountyEnum County { get; set; }
    }
}