using System.Collections.Generic;
using System.Threading.Tasks;
using projectDemo.Common.PageRequest;
using projectDemo.DTO.Response;
using projectDemo.DTO.Response.Promotion;
using projectDemo.Entity.Enum;
using projectDemo.Entity.Models;

namespace projectDemo.Repository.PromotionRepository
{
    public interface IPromotionRepository
    {
        Task<List<Promotion>> GetAllAsync();
        Task<Promotion?> GetByIdAsync(int id);
        Task AddAsync(Promotion entity);
        void Update(Promotion entity);
        void Remove(Promotion entity);
        Task<Promotion> GetAllCodeName(string CodeName);

    }
}
