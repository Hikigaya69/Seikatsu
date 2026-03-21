
using Seikatsu.Backend.Exceptions;

namespace Seikatsu.Backend.Middleware
{
    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // run the entire request pipeline normally
                // controller → service → database
                await next(context);
            }
            catch (AppException ex)
            {
                // known domain error — wrong password, not found, conflict etc.
                // logged as Warning because it is expected, not a crash
                _logger.LogWarning("Domain error [{ErrorCode}] {StatusCode}: {Message}",
                    ex.ErrorCode,
                    ex.StatusCode,
                    ex.Message);

                await WriteResponse(context, ex.StatusCode, ex.ErrorCode, ex.Message, ex.Errors);
            }
            catch (Exception ex)
            {
                // unexpected crash — database down, null reference, out of memory etc.
                // logged as Error because it needs investigation
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

                await WriteResponse(context, 500, "INTERNAL_ERROR", "An unexpected error occurred.", null);
            }
        }

        private static Task WriteResponse(
            HttpContext context,
            int statusCode,
            string errorCode,
            string message,
            Dictionary<string, string>? errors)
        {
            // set the HTTP status code — 401, 404, 409, 500 etc.
            context.Response.StatusCode = statusCode;

            // tell the client the body is JSON
            context.Response.ContentType = "application/json";

            // build and write the response body
            return context.Response.WriteAsJsonAsync(new
            {
                status = statusCode,
                data = (object?)null,
                errorCode = errorCode,
                message = message,
                errors = errors   // null for most exceptions, populated for VALIDATION_ERROR
            });
        }
    }
}
