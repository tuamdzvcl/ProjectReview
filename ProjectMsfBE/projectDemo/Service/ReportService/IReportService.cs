using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;

namespace projectDemo.Service.ReportService
{
    public interface IReportService
    {
        Task<ApiResponse<ReportResponse>> GetRevenueReportAsync(Guid userId, ReportRequest request);
    }
}
