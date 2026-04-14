using projectDemo.DTO.Projection;
using projectDemo.DTO.Request;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Entity.Models;

namespace projectDemo.Repository.PemisstionRepository
{
    public interface IPemisstionRepository
    {
        Task<Permissions> Create(Permissions entity);
        string Delete(int permissionID);
        Task<Permissions> GetByID(int permissionID);
        Task<(RoleProjecttion, int status, string messger)> GetPermissionByRoleId(int RoleId);
    }
}
