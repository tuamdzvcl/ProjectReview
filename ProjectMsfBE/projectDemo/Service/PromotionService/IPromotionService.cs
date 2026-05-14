using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using projectDemo.DTO.Request.Promotion;
using projectDemo.DTO.Response.Promotion;
using projectDemo.DTO.UpdateRequest.Promotion;
using projectDemo.DTO.Respone;

namespace projectDemo.Service.PromotionService
{
    public interface IPromotionService
    {
        Task<PageResponse<PromotionResponse>> GetAllPromotionsAsync(PromotionQuery query);
        Task<ApiResponse<PromotionResponse>> GetPromotionByIdAsync(int id);
        Task<ApiResponse<PromotionResponse>> CreatePromotionAsync(PromotionCreateRequest request,Guid userid);
        Task<ApiResponse<PromotionResponse>> UpdatePromotionAsync(int id, PromotionUpdateRequest request);
        Task<ApiResponse<bool>> DeletePromotionAsync(int id);
        Task<ApiResponse<string>> ImportPromotionsAsync(IFormFile file);
        Task<byte[]> ExportPromotionsAsync();
        Task<byte[]> DownloadTemplateAsync(Guid userid);
    }
}
