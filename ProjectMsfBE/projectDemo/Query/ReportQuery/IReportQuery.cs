using projectDemo.DTO.Query;

namespace projectDemo.Repository.ReportQuery
{
    public interface IReportQuery
    {
        Task<List<RevenueReportFlatRow>> GetRevenueRowsAsync(
            Guid userId,
            DateTime fromDate,
            DateTime toDateExclusive
        );

        Task<List<RevenueReportFlatRow>> GetPlatformRevenueRowsAsync(
            DateTime fromDate,
            DateTime toDateExclusive
        );

        Task<List<RevenueReportFlatRow>> GetUpgradeRowsAsync(
            DateTime fromDate,
            DateTime toDateExclusive
        );
    }
}
