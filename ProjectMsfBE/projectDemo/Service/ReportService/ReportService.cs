using System.Globalization;
using projectDemo.DTO.Query;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Enum;
using projectDemo.Repository.ReportQuery;

namespace projectDemo.Service.ReportService
{
    public class ReportService : IReportService
    {
        private readonly IReportQuery _reportQuery;

        public ReportService(IReportQuery reportQuery)
        {
            _reportQuery = reportQuery;
        }

        public async Task<ApiResponse<ReportResponse>> GetRevenueReportAsync(
            Guid userId,
            ReportRequest request
        )
        {
            string groupBy = (request.GroupBy ?? "monthly").Trim().ToLower();

            // 1. Xác định khoảng thời gian báo cáo
            DateTime fromDate;
            DateTime toDate;

            if (groupBy == "yearly")
            {
                // Mặc định lấy cả năm từ 1/1 đến 31/12
                int year = request.FromDate?.Year ?? DateTime.Today.Year;
                fromDate = new DateTime(year, 1, 1);
                toDate = new DateTime(year, 12, 31);
            }
            else
            {
                if (!request.FromDate.HasValue || !request.ToDate.HasValue)
                {
                    return ApiResponse<ReportResponse>.FailResponse(
                        EnumStatusCode.BAD_REQUEST,
                        "Vui lòng chọn Ngày bắt đầu và Ngày kết thúc."
                    );
                }

                fromDate = request.FromDate.Value.Date;
                toDate = request.ToDate.Value.Date;

                if (fromDate > toDate)
                {
                    return ApiResponse<ReportResponse>.FailResponse(
                        EnumStatusCode.BAD_REQUEST,
                        "Ngày bắt đầu không được lớn hơn ngày kết thúc."
                    );
                }

                if (groupBy == "daily" && (toDate - fromDate).TotalDays > 31)
                {
                    return ApiResponse<ReportResponse>.FailResponse(
                        EnumStatusCode.BAD_REQUEST,
                        "Báo cáo theo ngày chỉ hỗ trợ tối đa 1 tháng."
                    );
                }
            }

            // 2. Lấy dữ liệu và tính toán Summary (Kỳ hiện tại và Kỳ trước để tính tăng trưởng)
            var totalDays = (toDate - fromDate).Days + 1;
            var previousFromDate = fromDate.AddDays(-totalDays);
            var previousToDate = fromDate; // Exclusive boundary

            var currentRows = await _reportQuery.GetRevenueRowsAsync(
                userId,
                fromDate,
                toDate.AddDays(1)
            );
            var previousRows = await _reportQuery.GetRevenueRowsAsync(
                userId,
                previousFromDate,
                previousToDate
            );

            var summary = CalculateSummary(currentRows, previousRows);

            // 3. Xây dựng dữ liệu biểu đồ (Chart) có bù đắp ngày/tháng trống
            var chart = BuildChart(currentRows, groupBy, fromDate, toDate);

            var response = new ReportResponse
            {
                Summary = summary,
                Chart = chart,
                Meta = new RevenueMetaDto
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    GroupBy = groupBy,
                },
            };

            return ApiResponse<ReportResponse>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                response,
                "Lấy báo cáo doanh thu thành công"
            );
        }

        private RevenueSummaryDto CalculateSummary(
            List<RevenueReportFlatRow> current,
            List<RevenueReportFlatRow> previous
        )
        {
            var curRevenue = current.Sum(x => x.Revenue);
            var preRevenue = previous.Sum(x => x.Revenue);
            var curOrders = current.Select(x => x.OrderId).Distinct().Count();
            var preOrders = previous.Select(x => x.OrderId).Distinct().Count();
            var curTickets = current.Sum(x => x.TicketQuantity);
            var preTickets = previous.Sum(x => x.TicketQuantity);

            return new RevenueSummaryDto
            {
                TotalRevenue = curRevenue,
                TotalOrders = curOrders,
                TotalTickets = curTickets,
                GrowthRevenue = CalculateGrowth(curRevenue, preRevenue),
                GrowthOrders = CalculateGrowth(curOrders, preOrders),
                GrowthTickets = CalculateGrowth(curTickets, preTickets),
            };
        }

        private List<RevenueChartDto> BuildChart(
            List<RevenueReportFlatRow> rows,
            string groupBy,
            DateTime from,
            DateTime to
        )
        {
            var result = new List<RevenueChartDto>();

            if (groupBy == "yearly")
            {
                var dataDict = rows.GroupBy(x => x.PaidDate.Month)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Revenue));
                for (int m = 1; m <= 12; m++)
                {
                    result.Add(
                        new RevenueChartDto
                        {
                            Time = new DateTime(from.Year, m, 1),
                            Label = $"Tháng {m}",
                            Revenue = dataDict.GetValueOrDefault(m, 0),
                        }
                    );
                }
            }
            else if (groupBy == "monthly")
            {
                var dataDict = rows.GroupBy(x => new DateTime(x.PaidDate.Year, x.PaidDate.Month, 1))
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Revenue));

                DateTime current = new DateTime(from.Year, from.Month, 1);
                DateTime endMonth = new DateTime(to.Year, to.Month, 1);

                while (current <= endMonth)
                {
                    result.Add(
                        new RevenueChartDto
                        {
                            Time = current,
                            Label = current.ToString("MM/yyyy"),
                            Revenue = dataDict.GetValueOrDefault(current, 0),
                        }
                    );
                    current = current.AddMonths(1);
                }
            }
            else // daily
            {
                var dataDict = rows.GroupBy(x => x.PaidDate.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Revenue));
                for (DateTime d = from; d <= to; d = d.AddDays(1))
                {
                    result.Add(
                        new RevenueChartDto
                        {
                            Time = d,
                            Label = d.ToString("dd/MM"),
                            Revenue = dataDict.GetValueOrDefault(d, 0),
                        }
                    );
                }
            }

            return result;
        }

        private double CalculateGrowth(decimal current, decimal previous)
        {
            if (previous == 0)
                return current > 0 ? 100 : 0;
            return (double)Math.Round((current - previous) / previous * 100, 2);
        }

        private double CalculateGrowth(int current, int previous)
        {
            if (previous == 0)
                return current > 0 ? 100 : 0;
            return Math.Round((double)(current - previous) / previous * 100, 2);
        }
    }
}
