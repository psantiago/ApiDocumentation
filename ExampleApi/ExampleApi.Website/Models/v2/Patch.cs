using System.ComponentModel.DataAnnotations;

namespace ExampleApi.Website.Models.v2
{
    public class Patches
    {
        public Patches(params Patch[] patches)
        {
            PatchList = patches;
        }

        [Required]
        public Patch[] PatchList { get; private set; } 
    }
    public class Patch
    {
        [Required]
        public string Op { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Value { get; set; }
    }
}