using System;
using System.Threading.Tasks;
using projectDemo.Entity.Models;

namespace projectDemo.Repository.UserUpgradeRepository
{
    public interface IUserUpgradeRepository
    {
        Task AddAsync(UserUpgrade userUpgrade);
        Task<UserUpgrade?> GetByIdAsync(Guid id);
        Task<UserUpgrade?> GetByIdWithUpgradeAsync(Guid id);
        Task<UserUpgrade?> GetActiveUpgradeByUserIdAsync(Guid userId);
        void Update(UserUpgrade userUpgrade);
    }
}
