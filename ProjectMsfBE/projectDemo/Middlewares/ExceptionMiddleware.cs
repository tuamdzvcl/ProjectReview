using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using projectDemo.DTO.Respone;
using projectDemo.Entity.Enum;
using projectDemo.Exceptions;

namespace projectDemo.Middlewares
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
                await HandleException(context, ex, _logger);
            }
        }

        private static Task HandleException(
            HttpContext context,
            Exception exception,
            ILogger logger
        )
        {
            HttpStatusCode statusCode;
            string message;
            EnumStatusCode errorCode;

            switch (exception)
            {
                case UnauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Bạn chưa đăng nhập hoặc token không hợp lệ";
                    errorCode = EnumStatusCode.UNAUTHORIZED;
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    errorCode = EnumStatusCode.BAD_REQUEST;
                    break;

                case BadHttpRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Invalid JSON format";
                    errorCode = EnumStatusCode.BAD_REQUEST;
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = "Không tìm thấy dữ liệu";
                    errorCode = EnumStatusCode.NOT_FOUND;
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "Internal Server Error";
                    errorCode = EnumStatusCode.SERVER;
                    break;
            }

            // log
            logger.LogError(exception, "Exception xảy ra");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR:");
            Console.WriteLine(exception);
            Console.ResetColor();

            var response = ApiResponse<object>.FailResponse(errorCode, message: message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            return context.Response.WriteAsync(json);
        }
    }
}
