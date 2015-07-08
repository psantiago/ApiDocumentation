using System.ComponentModel.DataAnnotations;

namespace ExampleApi.Website.Models.v2
{

    public class RequestErrors
    {
        public RequestErrors(params Error[] errors)
        {
            Errors = errors;
        }

        [Required]
        public Error[] Errors { get; private set; } 
    }


    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public Error(string message, string property) : this(message)
        {
            Property = property;
        }

        [Required]
        public string Property { get; private set; }

        [Required]
        public string Message { get; private set; }
    }
}