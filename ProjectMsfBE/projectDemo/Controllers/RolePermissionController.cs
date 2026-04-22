using Microsoft.AspNetCore.Mvc;
using projectDemo.DTO.Request;
using projectDemo.Service.PermissionService;

namespace projectDemo.Controllers
{
    [Route("api/role-permission")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListPermissionRole()
        {
            var result = await _rolePermissionService.GetListPermissionRole();
            return Ok(result);
        }

        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetPermissionByRoleId(int roleId)
        {
            var result = await _rolePermissionService.GetPermissionByRoleId(roleId);
            return Ok(result);
        }

        [HttpPut("{roleid}")]
        public async Task<IActionResult> Update(RolePermissionResquest resquest,int roleid)
        {
            var result = await _rolePermissionService.UpdateRolePermission(resquest,roleid);
            return Ok(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(RolePermissionResquest resquest)
        {
            var result = await _rolePermissionService.CreateRolePermission(resquest);
            return Ok(result);
        }
    }
}
