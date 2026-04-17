using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using projectDemo.Data;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.UpgradeRepository
{
    public class UpgradeRepository : RepositoryLinqBase<Upgrade>, IUpgradeRepository
    {
        public UpgradeRepository(IUnitOfWork uow) : base(uow)
        {
        }

        public async Task<Upgrade?> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
