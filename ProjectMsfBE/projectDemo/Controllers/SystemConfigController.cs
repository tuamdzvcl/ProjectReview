using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectDemo.Service.ConfigService;

namespace projectDemo.Controllers
{
    [ApiController]
    [Route("api/system-config")]
    [AllowAnonymous] 
    public class SystemConfigController : ControllerBase {
        private readonly ISystemConfigService _configService;

        public SystemConfigController(ISystemConfigService configService) {
            _configService = configService;
        }

        [HttpGet]
        public IActionResult GetConfig() {
            return Ok(_configService.GetSystemConfig());
        }
    }
}
