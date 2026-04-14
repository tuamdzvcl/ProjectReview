using System.Net;
using projectDemo.Entity.Enum;

namespace projectDemo.DTO.Respone
{
    public class ApiResponse<T>
    {
        public EnumStatusCode StatusCode { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public object? Errors { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> SuccessResponse(
            EnumStatusCode statuscode,
            T data,
            string message = "Success"
        )
        {
            return new ApiResponse<T>
            {
                StatusCode = statuscode,
                Success = true,
                Message = message,
                Data = data,
            };
        }

        public static ApiResponse<T> FailResponse(
            EnumStatusCode statuscode,
            string message,
            object? errors = null
        )
        {
            return new ApiResponse<T>
            {
                StatusCode = statuscode,
                Success = false,
                Message = message,
                Errors = errors,
            };
        }
    }
}
