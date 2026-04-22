using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectDemo.DTO.Request.Upgrade;
using projectDemo.DTO.Response.Upgrade;
using projectDemo.DTO.UpdateRequest.Upgrade;
using projectDemo.Service.UpgradeService;

namespace projectDemo.Controllers
{
    [Authorize]
    [Route("api/upgrade")]
    [ApiController]
    public class UpgradeController : ControllerBase
    {
        private readonly IUpgradeService _upgradeService;

        public UpgradeController(IUpgradeService upgradeService)
        {
            _upgradeService = upgradeService;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] UpgradeQuery query)
        {
            var result = await _upgradeService.GetAllUpgradesAsync(query);
            return Ok(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Register(UpdateUserToOrganizer request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var result = await _upgradeService.RegisterUpgradePackageAsync(userId, request.id);
            return Ok(result);
        }

        [HttpGet("my-current-package")]
        public async Task<IActionResult> GetMyPackage()
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var result = await _upgradeService.GetCurrentUserUpgradeAsync(userId);
            return Ok(result);
        }

        // ADMIN ENDPOINTS
        [HttpPost("admin")]
        public async Task<IActionResult> Create(UpgradeCreateRequest request)
        {
            var result = await _upgradeService.CreateUpgradeAsync(request);
            return Ok(result);
        }

        [HttpPut("admin/{id}")]
        public async Task<IActionResult> Update(int id, UpgradeUpdateRequest request)
        {
            var result = await _upgradeService.UpdateUpgradeAsync(id, request);
            return Ok(result);
        }

        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _upgradeService.GetUpgradeByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete("admin/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _upgradeService.DeleteUpgradeAsync(id);
            return Ok(result);
        }

        [HttpPost("admin/import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var result = await _upgradeService.ImportUpgradesAsync(file);
            return Ok(result);
        }

        [HttpGet("admin/export")]
        public async Task<IActionResult> Export()
        {
            var bytes = await _upgradeService.ExportUpgradesAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MembershipPackages.xlsx");
        }

        [HttpGet("admin/template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var bytes = await _upgradeService.DownloadTemplateAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MembershipTemplate.xlsx");
        }
    }
}
