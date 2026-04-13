using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using projectDemo.Data;
using projectDemo.Entity.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace projectDemo.Middlewares
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory)
        {
            // Bước 1: Request đi vào hệ thống
            var request = context.Request;
            
            // Chỉ log các route API để tránh rác log (tùy chọn, ở đây lọc theo /api)
            if (!request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            // Bước 2: Middleware ghi nhận thông tin ban đầu (URL, Method, Time)
            var path = request.Path.ToString();
            var method = request.Method;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var startTime = DateTime.UtcNow;

            // Bước 3: Cho request đi tiếp (Backend xử lý bình thường)
            await _next(context);

            // Bước 4 & 5: Sau khi xử lý xong (Response trả ra), middleware ghi nhật ký
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<EventTickDbContext>();

                var user = context.User;
                // Lấy User ID từ Claims chuẩn hoặc 'id' tùy theo token của dự án
                string? userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                    ?? user.FindFirst("id")?.Value;
                
                Guid? userId = null;
                if (Guid.TryParse(userIdStr, out var gId)) userId = gId;

                string? username = user.Identity?.Name ?? user.FindFirst("Email")?.Value;

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Username = username,
                    Method = method,
                    Path = path,
                    StatusCode = context.Response.StatusCode,
                    IpAddress = ipAddress,
                    Timestamp = startTime,
                    Note = $"User {username ?? "Anonymous"} executed {method} {path}"
                };

                dbContext.AuditLogs.Add(auditLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Tránh làm crash request nếu việc ghi log gặp lỗi
            }
        }
    }
}
