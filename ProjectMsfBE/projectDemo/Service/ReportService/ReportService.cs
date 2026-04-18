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
            var (fromDate, toDate, groupBy, error) = ValidateRequest(request);
            if (error != null) return error;

            var totalDays = (toDate - fromDate).Days + 1;
            var previousFromDate = fromDate.AddDays(-totalDays);
            var previousToDate = fromDate;

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

            return GenerateSuccessResponse(currentRows, previousRows, fromDate, toDate, groupBy);
        }

        public async Task<ApiResponse<ReportResponse>> GetPlatformRevenueReportAsync(ReportRequest request)
        {
            var (fromDate, toDate, groupBy, error) = ValidateRequest(request);
            if (error != null) return error;

            var totalDays = (toDate - fromDate).Days + 1;
            var previousFromDate = fromDate.AddDays(-totalDays);
            var previousToDate = fromDate;

            var currentRows = await _reportQuery.GetPlatformRevenueRowsAsync(
                fromDate,
                toDate.AddDays(1)
            );
            var previousRows = await _reportQuery.GetPlatformRevenueRowsAsync(
                previousFromDate,
                previousToDate
            );

            return GenerateSuccessResponse(currentRows, previousRows, fromDate, toDate, groupBy);
        }

        public async Task<ApiResponse<ReportResponse>> GetUpgradeReportAsync(ReportRequest request)
        {
            var (fromDate, toDate, groupBy, error) = ValidateRequest(request);
            if (error != null) return error;

            var totalDays = (toDate - fromDate).Days + 1;
            var previousFromDate = fromDate.AddDays(-totalDays);
            var previousToDate = fromDate;

            var currentRows = await _reportQuery.GetUpgradeRowsAsync(
                fromDate,
                toDate.AddDays(1)
            );
            var previousRows = await _reportQuery.GetUpgradeRowsAsync(
                previousFromDate,
                previousToDate
            );

            return GenerateSuccessResponse(currentRows, previousRows, fromDate, toDate, groupBy);
        }

        private (DateTime fromDate, DateTime toDate, string groupBy, ApiResponse<ReportResponse>? error) ValidateRequest(ReportRequest request)
        {
            string groupBy = (request.GroupBy ?? "monthly").Trim().ToLower();
            DateTime fromDate;
            DateTime toDate;

            if (groupBy == "yearly")
            {
                int year = request.FromDate?.Year ?? DateTime.Today.Year;
                fromDate = new DateTime(year, 1, 1);
                toDate = new DateTime(year, 12, 31);
            }
            else
            {
                if (!request.FromDate.HasValue || !request.ToDate.HasValue)
                {
                    return (default, default, groupBy, ApiResponse<ReportResponse>.FailResponse(EnumStatusCode.BAD_REQUEST, "Vui lòng chọn Ngày bắt đầu và Ngày kết thúc."));
                }
                fromDate = request.FromDate.Value.Date;
                toDate = request.ToDate.Value.Date;
                if (fromDate > toDate)
                {
                    return (default, default, groupBy, ApiResponse<ReportResponse>.FailResponse(EnumStatusCode.BAD_REQUEST, "Ngày bắt đầu không được lớn hơn ngày kết thúc."));
                }
                if (groupBy == "daily" && (toDate - fromDate).TotalDays > 31)
                {
                    return (default, default, groupBy, ApiResponse<ReportResponse>.FailResponse(EnumStatusCode.BAD_REQUEST, "Báo cáo theo ngày chỉ hỗ trợ tối đa 1 tháng."));
                }
            }
            return (fromDate, toDate, groupBy, null);
        }

        private ApiResponse<ReportResponse> GenerateSuccessResponse(List<RevenueReportFlatRow> currentRows, List<RevenueReportFlatRow> previousRows, DateTime fromDate, DateTime toDate, string groupBy)
        {
            var summary = CalculateSummary(currentRows, previousRows);
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

            return ApiResponse<ReportResponse>.SuccessResponse(EnumStatusCode.SUCCESS, response, "Lấy báo cáo thành công");
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
                var grouped = rows.GroupBy(x => x.PaidDate.Month)
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            Revenue = g.Sum(x => x.Revenue),
                            Tickets = g.Sum(x => x.TicketQuantity),
                        }
                    );
                for (int m = 1; m <= 12; m++)
                {
                    var data = grouped.GetValueOrDefault(m, new { Revenue = 0m, Tickets = 0 });
                    result.Add(
                        new RevenueChartDto
                        {
                            Time = new DateTime(from.Year, m, 1),
                            Label = $"Tháng {m}",
                            Revenue = data.Revenue,
                            Tickets = data.Tickets,
                        }
                    );
                }
            }
            else if (groupBy == "monthly")
            {
                var grouped = rows.GroupBy(x => new DateTime(x.PaidDate.Year, x.PaidDate.Month, 1))
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            Revenue = g.Sum(x => x.Revenue),
                            Tickets = g.Sum(x => x.TicketQuantity),
                        }
                    );

                DateTime current = new DateTime(from.Year, from.Month, 1);
                DateTime endMonth = new DateTime(to.Year, to.Month, 1);

                while (current <= endMonth)
                {
                    var data = grouped.GetValueOrDefault(
                        current,
                        new { Revenue = 0m, Tickets = 0 }
                    );
                    result.Add(
                        new RevenueChartDto
                        {
                            Time = current,
                            Label = current.ToString("MM/yyyy"),
                            Revenue = data.Revenue,
                            Tickets = data.Tickets,
                        }
                    );
                    current = current.AddMonths(1);
                }
            }
            else
            {
                var grouped = rows.GroupBy(x => x.PaidDate.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            Revenue = g.Sum(x => x.Revenue),
                            Tickets = g.Sum(x => x.TicketQuantity),
                        }
                    );
                for (DateTime d = from; d <= to; d = d.AddDays(1))
                {
                    var data = grouped.GetValueOrDefault(d, new { Revenue = 0m, Tickets = 0 });
                    result.Add(
                        new RevenueChartDto
                        {
                            Time = d,
                            Label = d.ToString("dd/MM"),
                            Revenue = data.Revenue,
                            Tickets = data.Tickets,
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
