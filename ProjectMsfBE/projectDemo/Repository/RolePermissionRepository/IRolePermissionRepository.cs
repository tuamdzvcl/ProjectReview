using projectDemo.DTO.Response;
using projectDemo.Entity.Models;

namespace projectDemo.Repository.RolePermissionRepository
{
    public interface IRolePermissionRepository
    {
        Task<List<PermissionResponse>> GetPermissionsbyRoleName(string roleName);
        Task<IEnumerable<PermisstionRoleResponse>> GetPermissionByRoleId(int RoleId);
        Task<List<PermisstionRoleResponse>> GetListPermissionRole();
        Task AddRangeAsync(List<RolePermissions> rolePermissions);
        void RemoveRange(IEnumerable<RolePermissions> entities);
        Task<List<RolePermissions>> GetByRoleIdAsync(int roleId);
    }
}
