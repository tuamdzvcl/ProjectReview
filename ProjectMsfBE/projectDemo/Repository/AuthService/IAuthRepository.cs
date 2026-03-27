using EventTick.Model.Models;
using projectDemo.DTO.Request;
using projectDemo.DTO.Response;

namespace projectDemo.Repository.Ipml
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<Guid> InsertAsync(User user);
        Task<List<PermissionResponse>> GetPermissionNameAsyncByUserId(Guid UserID);
        Task AddAsync();
        Task<List<PermissionResponse>> GetPermissionsbyRoleName(string roleName);
    }
}
