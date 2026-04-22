using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using projectDemo.DTO.Request.Upgrade;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response.Momo;
using projectDemo.DTO.Response.Upgrade;
using projectDemo.DTO.UpdateRequest.Upgrade;

namespace projectDemo.Service.UpgradeService
{
    public interface IUpgradeService
    {
        Task<PageResponse<UpgradeResponse>> GetAllUpgradesAsync(UpgradeQuery query);
        Task<ApiResponse<UpgradeResponse>> GetUpgradeByIdAsync(int id);
        Task<ApiResponse<UpgradeResponse>> CreateUpgradeAsync(UpgradeCreateRequest request);
        Task<ApiResponse<UpgradeResponse>> UpdateUpgradeAsync(int id, UpgradeUpdateRequest request);
        Task<ApiResponse<MomoCreatePaymentResponseModel>> RegisterUpgradePackageAsync(Guid userId, int upgradeId);
        Task<ApiResponse<UserUpgradeResponse>> GetCurrentUserUpgradeAsync(Guid userId);
        Task<ApiResponse<bool>> DeleteUpgradeAsync(int id);
        Task<ApiResponse<string>> ImportUpgradesAsync(IFormFile file);
        Task<byte[]> ExportUpgradesAsync();
        Task<byte[]> DownloadTemplateAsync();
    }
}
