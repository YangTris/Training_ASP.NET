using Core.Exceptions;
using System.Net;

namespace API.Middlewares
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode statusCode;
            string logMessage;

            switch (ex)
            {
                case BadRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    logMessage = "Bad Request: " + ex.Message;
                    _logger.LogError(ex, logMessage);
                    break;
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    logMessage = "Not Found: " + ex.Message;
                    _logger.LogError(ex, logMessage);
                    break;
                case DomainException:
                    statusCode = HttpStatusCode.UnprocessableEntity;
                    logMessage = "Unprocessable Entity: " + ex.Message;
                    _logger.LogError(ex, logMessage);
                    break;
                case ConflictException:
                    statusCode = HttpStatusCode.Conflict;
                    logMessage = "Conflict: " + ex.Message;
                    _logger.LogError(ex, logMessage);
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    logMessage = "Internal Server Error: " + ex.Message;
                    _logger.LogError(ex, logMessage);
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = context.Response.StatusCode,
                error = logMessage,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

}