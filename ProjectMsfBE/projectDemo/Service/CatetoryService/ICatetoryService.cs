using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Models;

namespace projectDemo.Service.CatetoryService
{
    public interface ICatetoryService
    {
        Task<ApiResponse<CatetoryResponse>> Create(CatetoryResquest resquest);
        Task<ApiResponse<CatetoryResponse>> Update(Guid id, CatetoryResquest resquest);
        Task<ApiResponse<string>> Delete(Guid id);
        Task<ApiResponse<CatetoryResponse>> Getbyid(Guid id);
        Task<PageResponse<CatetoryResponse>> GetCatetoryListEvent(
            int pageSize,
            int pageIndex,
            string key
        );
        Task<ApiResponse<List<CatetoryResponse>>> GetCatetory();
    }
}
