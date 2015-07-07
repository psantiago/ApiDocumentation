namespace ExampleApi.Client.Models
{
    public enum CountyEnum
    {
        Unknown = 0,
        Monongalia,
        Marion
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CountyEnum County { get; set; }
    }
}