using System.Net;
using System.Text.Json;
using CareSchedule.API.Contracts;

namespace CareSchedule.API.Middleware
{
    public class GlobalExceptionMiddleware(RequestDelegate _next, IHostEnvironment _env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Handle cases where the framework sets 401 or 403 without an exception
                if (!context.Response.HasStarted)
                {
                    if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                    {
                        await WriteResponse(context, HttpStatusCode.Forbidden, 
                            "FORBIDDEN", "You do not have the required permissions to access this resource.");
                    }
                    else if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                    {
                        await WriteResponse(context, HttpStatusCode.Unauthorized, 
                            "UNAUTHORIZED", "Authentication is required to access this resource.");
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                await WriteResponse(context, HttpStatusCode.NotFound,
                    "RESOURCE_NOT_FOUND", ex.Message);
            }
            catch (ArgumentException ex)
            {
                await WriteResponse(context, HttpStatusCode.BadRequest,
                    "BAD_REQUEST", ex.Message);
            }
            catch (Exception ex)
            {
                var message = _env.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occurred.";

                await WriteResponse(context, HttpStatusCode.InternalServerError,
                    "INTERNAL_ERROR", message);
            }
        }

        private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode,
            string errorCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.Fail(new { code = errorCode }, message);

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}