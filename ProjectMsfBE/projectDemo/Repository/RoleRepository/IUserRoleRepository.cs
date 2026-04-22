using EventTick.Model.Models;
using projectDemo.Entity.Models;

namespace projectDemo.Repository.Ipml
{
    public interface IUserRoleRepository
    {
        Task InsertAsync(UserRole userRole);
        Task InserList(List<UserRole> users);
        Task<UserRole> GetByIdAsync(int id);
        string DeleteByIdAsync(UserRole user);
        UserRole Update(UserRole user);

        Task<IEnumerable<Permissions>> GetAllByUserId(Guid userId);
    }
}
