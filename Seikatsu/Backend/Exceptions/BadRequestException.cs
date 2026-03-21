namespace Seikatsu.Backend.Exceptions
{
    public class BadRequestException :AppException
    {
        // for simple bad requests without validation errors
        public BadRequestException(string message)
            : base(message, 400, "BAD_REQUEST") { }
        // for bad requests with validation errors (e.g. form validation)
        public BadRequestException(Dictionary<string, string> errors)
            : base("Validation failed.", 400, "VALIDATION_ERROR", errors) { }
    }
}
