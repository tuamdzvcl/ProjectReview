using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.UserUpgradeRepository
{
    public class UserUpgradeRepository : RepositoryLinqBase<UserUpgrade>, IUserUpgradeRepository
    {
        public UserUpgradeRepository(IUnitOfWork uow)
            : base(uow) { }

        public async Task AddAsync(UserUpgrade userUpgrade)
        {
            await _dbSet.AddAsync(userUpgrade);
        }

        public async Task<UserUpgrade?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<UserUpgrade?> GetByIdWithUpgradeAsync(Guid id)
        {
            return await _dbSet.Include(u => u.Upgrade).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserUpgrade?> GetActiveUpgradeByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.Upgrade)
                .Where(u => u.UserId == userId && u.Status == "ACTIVE" && u.EndDate > DateTime.Now)
                .OrderByDescending(u => u.StartDate)
                .FirstOrDefaultAsync();
        }

        public void Update(UserUpgrade userUpgrade)
        {
            _dbSet.Update(userUpgrade);
        }
    }
}
