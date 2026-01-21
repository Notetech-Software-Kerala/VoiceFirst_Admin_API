using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VoiceFirst_Admin.Utilities.Exceptions;
using VoiceFirst_Admin.Utilities.Models.Common;

namespace VoiceFirst_Admin.Utilities.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception at {Path}", context.Request.Path);

                context.Response.ContentType = "application/json";

                ApiResponse<object> response;
                int statusCode;

                switch (ex)
                {
                    case BusinessNotFoundException notFound:
                        statusCode = StatusCodes.Status404NotFound;
                        response = ApiResponse<object>.Fail(
                            notFound.Message,
                            statusCode,
                            notFound.ErrorCode);
                        break;

                    case BusinessConflictException conflict:
                        statusCode = StatusCodes.Status409Conflict;
                        response = ApiResponse<object>.Fail(
                            conflict.Message,
                            statusCode,
                            conflict.ErrorCode);
                        break;

                    case BusinessRecoverableException conflict:
                        statusCode = StatusCodes.Status410Gone;
                        response = ApiResponse<object>.Fail(
                            conflict.Message,
                            statusCode,
                            conflict.ErrorCode);
                        break;

                    case BusinessValidationException validation:
                        statusCode = StatusCodes.Status400BadRequest;
                        response = ApiResponse<object>.Fail(
                            validation.Message,
                            statusCode,
                            validation.ErrorCode);
                        break;

                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        response = ApiResponse<object>.Fail(
                            "Something went wrong. Please try again later.",
                            statusCode);
                        break;
                }

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
