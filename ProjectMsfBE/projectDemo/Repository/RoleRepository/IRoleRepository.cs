using EventTick.Model.Enum;
using EventTick.Model.Models;
using projectDemo.DTO.Response;

namespace projectDemo.Repository.Ipml;

public interface IRoleRepository
{
    Task<int> GetOrCreateAsync(Role roleName);
    Role Update(Role role);

    Task<List<Role>> GetListRoleById(int id);
    string DeleteAsync(Role role);
    Task<Role> GetRole(string roleName);

    Task<Role> GetByid(int roleid);
    Task<List<PermisstionRoleResponse>> GetRoleListPermisson();
}
