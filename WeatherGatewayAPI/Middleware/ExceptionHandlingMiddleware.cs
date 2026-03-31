using System.Net;
using System.Text.Json;

namespace WeatherGatewayAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) 
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) 
        {
            try 
            {
                await _next(context);
            }

            catch(TaskCanceledException ex) 
            {
                _logger.LogError(ex, "Request timed out.");

                await HandleExceptionAsync(context, HttpStatusCode.GatewayTimeout, "The request timed out.");
            }

            catch(HttpRequestException ex) 
            {
                _logger.LogError(ex, "External HTTP request failed.");

                await HandleExceptionAsync(context, HttpStatusCode.ServiceUnavailable, "External service is unavailable.");
            }

            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message) 
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                message = message
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
