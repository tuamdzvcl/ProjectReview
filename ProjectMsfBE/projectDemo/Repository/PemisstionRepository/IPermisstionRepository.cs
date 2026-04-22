using projectDemo.DTO.Projection;
using projectDemo.DTO.Request;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Entity.Models;

namespace projectDemo.Repository.PemisstionRepository
{
    public interface IPermisstionRepository
    {
        Task<Permissions> Create(Permissions entity);
        string Delete(int permissionID);
        Task<Permissions> GetByID(int permissionID);
        Task<IEnumerable<Permissions>> GetAllAsync();
        Task<(RoleProjecttion, int status, string messger)> GetPermissionByRoleId(int RoleId);
    }
}
