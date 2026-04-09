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
        public async Task<IActionResult> GetRevenueReport([FromQuery] ReportRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var result = await _reportService.GetRevenueReportAsync(userId, request);
            return Ok(result);
        }
    }
}
