using EventTick.Model.Enum;
using EventTick.Model.Models;

namespace projectDemo.Repository.Ipml;

public interface IRoleRepository
{
    Task<int> GetOrCreateAsync(Role roleName);
    Role Update(Role role);
    string   DeleteAsync(Role role);
    Task<Role> GetRole(string roleName);

    Task<Role> GetByid(int roleid);
    
}
