using Dapper;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Data;
using projectDemo.Repository.BaseData;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository
{
    public class UserLoginRepository : RepositoryLinqBase<UserLogin>, IUserLoginRepository
    {
        private readonly RepositoryProcBase _proc;

        public UserLoginRepository(IUnitOfWork uow)
            : base(uow)
        {
            _proc = new RepositoryProcBase(uow);
        }

        public string DeleteAsync(UserLogin userLogin)
        {
            _dbSet.Remove(userLogin);
            return "Deleted";
        }

        public async Task<UserLogin?> getbyid(int user)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == user && x.IsDeleted == false);
        }

        public async Task InsertAsync(UserLogin userLogin)
        {
            await _dbSet.AddAsync(userLogin);
        }

        public UserLogin Update(UserLogin userLogin)
        {
            _dbSet.Update(userLogin);
            return userLogin;
        }

        public async Task<UserLogin?> GetByProviderUserIdAsync(
            string providerUserId,
            string Provider
        )
        {
            return await _dbSet
                .Where(x => x.ProviderUserId == providerUserId && x.Provider == Provider)
                .FirstOrDefaultAsync();
        }
    }
}
