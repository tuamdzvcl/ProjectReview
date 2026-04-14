using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Models;

namespace projectDemo.Repository.CatetoryRepository
{
    public interface ICatetoryReposioty
    {
        Task<Catetory> Create(Catetory entity);
        Task<string> delete(Guid id);

        Task<Catetory> GetbyId(Guid id);
        Task<List<Catetory>> GetListCatetory();

        Task<Catetory> GetByName(string name);
        Task<PageResponse<CatetoryResponse>> PageCatetoryEvent(
            int PageSize,
            int PageIndex,
            string key
        );
    }
}
