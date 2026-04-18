using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectDemo.DTO.Request;
using projectDemo.Service.ReportService;

namespace projectDemo.Controllers
{
    [Route("api/report")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport([FromQuery] ReportRequest request, [FromQuery] bool all = false)
        {
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var isAdmin = userRole?.ToUpper() == "ADMIN";

            if (all && isAdmin)
            {
                var resultAll = await _reportService.GetPlatformRevenueReportAsync(request);
                return Ok(resultAll);
            }

            var userId = Guid.Parse(User.FindFirst("id").Value);
            var result = await _reportService.GetRevenueReportAsync(userId, request);
            return Ok(result);
        }

        [HttpGet("upgrades")]
        public async Task<IActionResult> GetUpgradeReport([FromQuery] ReportRequest request)
        {
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var isAdmin = userRole?.ToUpper() == "ADMIN";

            if (!isAdmin)
            {
                return Forbid();
            }

            var result = await _reportService.GetUpgradeReportAsync(request);
            return Ok(result);
        }
    }
}
