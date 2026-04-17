using System.Collections.Generic;
using System.Threading.Tasks;
using projectDemo.Entity.Models;

namespace projectDemo.Repository.UpgradeRepository
{
    public interface IUpgradeRepository
    {
        Task<List<Upgrade>> GetAllAsync();
        Task<Upgrade?> GetById(int id);
        Task AddAsync(Upgrade entity);
        void Update(Upgrade entity);
        void Remove(Upgrade entity);
    }
}
