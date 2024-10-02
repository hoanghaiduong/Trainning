

using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace Trainning.Common.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            catch (SqlException sqlEx)
            {
                _logger.LogError($"SQL Exception: {sqlEx.Message}");
                await HandleExceptionAsync(context, sqlEx, HttpStatusCode.InternalServerError,sqlEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(result);
        }
    }
}