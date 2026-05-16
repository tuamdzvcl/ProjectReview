using System.Collections.Generic;
using System.Threading.Tasks;
using EventTick.Model.Enum;
using Microsoft.EntityFrameworkCore;
using projectDemo.Common;
using projectDemo.Common.PageRequest;
using projectDemo.Data;
using projectDemo.DTO.Response;
using projectDemo.DTO.Response.Promotion;
using projectDemo.Entity.Enum;
using projectDemo.Entity.Models;
using projectDemo.Repository.BaseData;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.PromotionRepository
{
    public class PromotionRepository : RepositoryLinqBase<Promotion>, IPromotionRepository
    {
        private readonly RepositoryProcBase _procBase;
        public PromotionRepository(IUnitOfWork uow) : base(uow)
        {
            _procBase = new RepositoryProcBase(uow);
        }

        public async Task<Promotion?> GetAllCodeName(string CodeName)
        {
            var now = DateTime.Now;
          return await  _dbSet
                .Where(x=>x.Code== CodeName && x.IsDeleted==false && x.EndDate >= now && x.StartDate<=now)
                .FirstOrDefaultAsync();

                
        }

        public async Task<Promotion?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
            
        }

        
    }
}
