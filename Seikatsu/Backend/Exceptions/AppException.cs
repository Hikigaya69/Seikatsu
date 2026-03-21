namespace Seikatsu.Backend.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public Dictionary<string, string>? Errors { get; }

        // simple — just message, used by most exceptions
        public AppException(string message, int statusCode, string errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        // with validation errors — used by BadRequestException for form validation
        public AppException(string message, int statusCode, string errorCode, Dictionary<string, string> errors) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Errors = errors;
        }
    }
}