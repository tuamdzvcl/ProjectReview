using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using projectDemo.Data;
using projectDemo.Entity.Models;

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
            var request = context.Request;

            if (!request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            var path = request.Path.ToString();
            var method = request.Method;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var startTime = DateTime.UtcNow;

            await _next(context);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<EventTickDbContext>();

                var user = context.User;
                string? userIdStr =
                    user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("id")?.Value;

                Guid? userId = null;
                if (Guid.TryParse(userIdStr, out var gId))
                    userId = gId;

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
                    Note = $"User {username ?? "Anonymous"} executed {method} {path}",
                };

                dbContext.AuditLogs.Add(auditLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception) { }
        }
    }
}
