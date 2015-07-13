using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace ExampleApi.Website.Models.v2
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Role
    {
        Unknown = 0,
        Admin,
        User,
        Guest
    }


    public class AppUsers
    {
        [Required]
        public int AppId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}