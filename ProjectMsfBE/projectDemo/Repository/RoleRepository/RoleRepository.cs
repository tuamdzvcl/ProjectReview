using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using projectDemo.Data;
using projectDemo.Repository.BaseData;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository
{
    public class RoleRepository : RepositoryLinqBase<Role>, IRoleRepository
    {
        private readonly RepositoryProcBase _procBase;

        public RoleRepository(IUnitOfWork uow)
            : base(uow)
        {
            _procBase = new RepositoryProcBase(uow);
        }

        public string DeleteAsync(Role role)
        {
            _dbSet.Remove(role);
            return "Deleted";
        }

        public async Task<Role?> GetByid(int roleid)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == roleid && x.IsDeleted == false);
        }

        public async Task<int> GetOrCreateAsync(Role role)
        {
            await _dbContext.AddAsync(role);
            return role.Id;
        }

        public async Task<Role?> GetRole(string roleName)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.RoleName == roleName);
        }

        public Role Update(Role role)
        {
            _dbSet.Update(role);
            return role;
        }
    }
}
