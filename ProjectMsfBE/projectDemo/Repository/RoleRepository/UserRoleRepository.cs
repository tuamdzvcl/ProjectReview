using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Data;
using projectDemo.Repository.BaseData;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository
{
    public class UserRoleRepository : RepositoryLinqBase<UserRole>, IUserRoleRepository
    {
        private readonly RepositoryProcBase _proc;
        public UserRoleRepository(IUnitOfWork uow) : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public string DeleteByIdAsync(UserRole role)
        {
            _dbSet.Remove(role);
            return "Deleted";
            
        }

        public async Task<UserRole?> GetByIdAsync(int id)
        {
           return await _dbSet.FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task InserList(List<UserRole> users)
        {
           await _dbSet.AddRangeAsync(users);
        }

        public async Task InsertAsync(UserRole userRole)
        {
            await _dbSet.AddAsync(userRole);
        }

        public UserRole Update(UserRole user)
        {
             _dbSet.Update(user);
            return user;
        }
    }
}
