using ConstructionManager.Api.Domain.Exceptions;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace ConstructionManager.Api.Presentation.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
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
            var (statusCode, message) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, ex.Message),
                DuplicateException => (HttpStatusCode.Conflict, ex.Message),
                ValidationException => (HttpStatusCode.BadRequest, ex.Message),
                BusinessRuleException => (HttpStatusCode.Conflict, ex.Message),
                OperationNotAllowedException => (HttpStatusCode.Conflict, ex.Message),
                NpgsqlException => (HttpStatusCode.ServiceUnavailable, "Database is unavailable"),
                DbUpdateException => (HttpStatusCode.InternalServerError, "Database operation failed"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = (int)statusCode,
                Message = message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}