using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectDemo.DTO.Request;
using projectDemo.Service.PermissionService;

namespace projectDemo.Controllers
{
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermisstionRequest request)
        {
            var result = await _permissionService.Create(request);
            return Ok(result);
        }

        [HttpDelete("{permissionID}")]
        public async Task<IActionResult> Delete(int permissionID)
        {
            var result = await _permissionService.Delete(permissionID);
            return Ok(result);
        }

        [HttpGet("{permissionID}")]
        public async Task<IActionResult> GetByID(int permissionID)
        {
            var result = await _permissionService.GetByID(permissionID);
            return Ok(result);
            ;
        }
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var result = await _permissionService.GetListPermission();
            return Ok(result);
            ;
        }

        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetByRole(int roleId)
        {
            var result = await _permissionService.GetByrole(roleId);
            return Ok(result);
            ;
        }
       
    }
}
