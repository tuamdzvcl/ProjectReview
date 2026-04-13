using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectDemo.Data;
using projectDemo.DTO.Respone;
using projectDemo.Entity.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace projectDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Chỉ ADMIN mới nên xem được (có thể thêm phân quyền chi tiết sau)
    public class AuditLogController : ControllerBase
    {
        private readonly EventTickDbContext _context;

        public AuditLogController(EventTickDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? username = null,
            [FromQuery] string? path = null)
        {
            try
            {
                var query = _context.AuditLogs.AsQueryable();

                if (!string.IsNullOrEmpty(username))
                {
                    query = query.Where(l => l.Username != null && l.Username.Contains(username));
                }

                if (!string.IsNullOrEmpty(path))
                {
                    query = query.Where(l => l.Path.Contains(path));
                }

                var totalRecords = await query.CountAsync();
                var logs = await query
                    .OrderByDescending(l => l.Timestamp)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new
                {
                    TotalRecords = totalRecords,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Items = logs
                };

                return Ok(ApiResponse<object>.SuccessResponse(projectDemo.Entity.Enum.EnumStatusCode.SUCCESS, response));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<object>.FailResponse(projectDemo.Entity.Enum.EnumStatusCode.SERVER, "Lỗi khi lấy nhật ký: " + ex.Message));
            }
        }
    }
}
