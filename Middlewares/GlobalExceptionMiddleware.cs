using AcademicAppoinment.Helpers.Exceptions;
using System.Net;

namespace AcademicAppoinment.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, string.IsNullOrWhiteSpace(exception.Message) ? "Bạn không có quyền truy cập." : exception.Message),
                ForbiddenAccessException => (HttpStatusCode.Forbidden, exception.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
                _ => (HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {exception.Message}{(exception.InnerException != null ? " | Inner: " + exception.InnerException.Message : "")}")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                statusCode = (int)statusCode,
                message = message,
                timestamp = DateTime.UtcNow
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
