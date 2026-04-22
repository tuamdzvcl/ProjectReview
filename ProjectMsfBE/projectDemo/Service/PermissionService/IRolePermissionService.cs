using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;

namespace projectDemo.Service.PermissionService
{
    public interface IRolePermissionService
    {
        Task<ApiResponse<List<PermisstionRoleResponse>>> GetListPermissionRole();
        Task<ApiResponse<IEnumerable<PermisstionRoleResponse>>> GetPermissionByRoleId(int roleId);

        Task<ApiResponse<string>> CreateRolePermission(RolePermissionResquest request);
        Task<ApiResponse<string>> UpdateRolePermission(RolePermissionResquest request,int roleid);
    }
}
