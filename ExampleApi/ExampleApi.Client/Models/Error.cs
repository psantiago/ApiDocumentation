namespace ExampleApi.Client.Models
{

    public class RequestErrors
    {
        public RequestErrors(params Error[] errors)
        {
            Errors = errors;
        }
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

        public string Property { get; private set; }
        public string Message { get; private set; }
    }
}