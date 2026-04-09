using projectDemo.DTO.Query;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Enum;
using projectDemo.Repository.ReportQuery;
using System.Globalization;

namespace projectDemo.Service.ReportService
{
    public class ReportService : IReportService
    {
        private readonly IReportQuery _reportQuery;

        public ReportService(IReportQuery reportQuery)
        {
            _reportQuery = reportQuery;
        }

        public async Task<ApiResponse<ReportResponse>> GetRevenueReportAsync(Guid userId, ReportRequest request)
        {
            var groupBy = NormalizeGroupBy(request.GroupBy);
            var usesCustomDateRange = string.IsNullOrWhiteSpace(groupBy);

            DateTime fromDate;
            DateTime toDate;
            DateTime previousFromDate;
            DateTime previousToDateExclusive;
            List<RevenueReportFlatRow> currentRows;
            List<RevenueReportFlatRow> previousRows;

            if (usesCustomDateRange)
            {
                if (!request.FromDate.HasValue || !request.ToDate.HasValue)
                {
                    return ApiResponse<ReportResponse>.FailResponse(
                        EnumStatusCode.BAD_REQUEST,
                        "Khi không có GroupBy thì phải truyền FromDate và ToDate");
                }

                fromDate = request.FromDate.Value.Date;
                toDate = request.ToDate.Value.Date;

                if (fromDate > toDate)
                {
                    return ApiResponse<ReportResponse>.FailResponse(
                        EnumStatusCode.BAD_REQUEST,
                        "FromDate không được lớn hơn ToDate");
                }

                var toDateExclusive = toDate.AddDays(1);
                var totalDays = (toDate - fromDate).Days + 1;
                previousFromDate = fromDate.AddDays(-totalDays);
                previousToDateExclusive = fromDate;

                currentRows = await _reportQuery.GetRevenueRowsAsync(userId, fromDate, toDateExclusive);
                previousRows = await _reportQuery.GetRevenueRowsAsync(userId, previousFromDate, previousToDateExclusive);
            }
            else
            {
                var allRows = await _reportQuery.GetRevenueRowsAsync(
                    userId,
                    new DateTime(1900, 1, 1),
                    new DateTime(2100, 1, 1));

                currentRows = allRows;
                previousRows = new List<RevenueReportFlatRow>();

                if (allRows.Any())
                {
                    fromDate = allRows.Min(x => x.PaidDate).Date;
                    toDate = allRows.Max(x => x.PaidDate).Date;
                }
                else
                {
                    fromDate = DateTime.Today;
                    toDate = DateTime.Today;
                }

                previousFromDate = fromDate;
                previousToDateExclusive = fromDate;
            }

            var currentRevenue = currentRows.Sum(x => x.Revenue);
            var previousRevenue = previousRows.Sum(x => x.Revenue);

            var currentOrders = currentRows.Select(x => x.OrderId).Distinct().Count();
            var previousOrders = previousRows.Select(x => x.OrderId).Distinct().Count();

            var currentTickets = currentRows.Sum(x => x.TicketQuantity);
            var previousTickets = previousRows.Sum(x => x.TicketQuantity);

            var response = new ReportResponse
            {
                Summary = new RevenueSummaryDto
                {
                    TotalRevenue = currentRevenue,
                    TotalOrders = currentOrders,
                    TotalTickets = currentTickets,
                    TotalViews = 0,
                    GrowthRevenue = usesCustomDateRange ? CalculateGrowth(currentRevenue, previousRevenue) : 0,
                    GrowthOrders = usesCustomDateRange ? CalculateGrowth(currentOrders, previousOrders) : 0,
                    GrowthTickets = usesCustomDateRange ? CalculateGrowth(currentTickets, previousTickets) : 0,
                    GrowthViews = 0
                },
                Chart = BuildChart(currentRows, groupBy, fromDate, toDate),
                Meta = new RevenueMetaDto
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    GroupBy = groupBy
                }
            };

            return ApiResponse<ReportResponse>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                response,
                "Lấy báo cáo doanh thu thành công");
        }

        private static string NormalizeGroupBy(string? groupBy)
        {
            if (string.IsNullOrWhiteSpace(groupBy))
            {
                return string.Empty;
            }

            var value = groupBy.Trim().ToLower();

            if (value == "monthly")
            {
                return "monthly";
            }

            if (value == "weekly")
            {
                return "weekly";
            }

            if (value == "daily" || value == "dailly")
            {
                return "dailly";
            }

            return string.Empty;
        }

        private static List<RevenueChartDto> BuildChart(
            List<RevenueReportFlatRow> rows,
            string groupBy,
            DateTime fromDate,
            DateTime toDate)
        {
            if (groupBy == "monthly")
            {
                var startMonth = new DateTime(fromDate.Year, fromDate.Month, 1);
                var endMonth = new DateTime(toDate.Year, toDate.Month, 1);

                var revenueByMonth = rows
                    .GroupBy(x => new DateTime(x.PaidDate.Year, x.PaidDate.Month, 1))
                    .ToDictionary(x => x.Key, x => x.Sum(v => v.Revenue));

                var result = new List<RevenueChartDto>();
                var currentMonth = startMonth;

                while (currentMonth <= endMonth)
                {
                    result.Add(new RevenueChartDto
                    {
                        Time = currentMonth,
                        Label = $"Tháng {currentMonth.Month}/{currentMonth.Year}",
                        Revenue = revenueByMonth.TryGetValue(currentMonth, out var revenue) ? revenue : 0
                    });

                    currentMonth = currentMonth.AddMonths(1);
                }

                return result;
            }

            if (groupBy == "weekly")
            {
                var startWeek = GetStartOfWeek(fromDate);
                var endWeek = GetStartOfWeek(toDate);

                var revenueByWeek = rows
                    .GroupBy(x => GetStartOfWeek(x.PaidDate))
                    .ToDictionary(x => x.Key, x => x.Sum(v => v.Revenue));

                var result = new List<RevenueChartDto>();
                var currentWeek = startWeek;

                while (currentWeek <= endWeek)
                {
                    var weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        currentWeek,
                        CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Monday);

                    result.Add(new RevenueChartDto
                    {
                        Time = currentWeek,
                        Label = $"Tuần {weekNumber}/{currentWeek.Year}",
                        Revenue = revenueByWeek.TryGetValue(currentWeek, out var revenue) ? revenue : 0
                    });

                    currentWeek = currentWeek.AddDays(7);
                }

                return result;
            }

            if (groupBy == "dailly")
            {
                var revenueByWeekday = rows
                    .GroupBy(x => NormalizeWeekday(x.PaidDate.DayOfWeek))
                    .ToDictionary(x => x.Key, x => x.Sum(v => v.Revenue));

                var orderedWeekdays = new[]
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday,
                    DayOfWeek.Sunday
                };

                return orderedWeekdays
                    .Select((day, index) => new RevenueChartDto
                    {
                        Time = fromDate.Date.AddDays(index),
                        Label = GetWeekdayLabel(day),
                        Revenue = revenueByWeekday.TryGetValue(day, out var revenue) ? revenue : 0
                    })
                    .ToList();
            }

            var revenueByDate = rows
                .GroupBy(x => x.PaidDate.Date)
                .ToDictionary(x => x.Key, x => x.Sum(v => v.Revenue));

            return Enumerable.Range(0, (toDate.Date - fromDate.Date).Days + 1)
                .Select(offset => fromDate.Date.AddDays(offset))
                .Select(day => new RevenueChartDto
                {
                    Time = day,
                    Label = day.ToString("dd/MM/yyyy"),
                    Revenue = revenueByDate.TryGetValue(day, out var revenue) ? revenue : 0
                })
                .ToList();
        }

        private static DateTime GetStartOfWeek(DateTime date)
        {
            var normalized = date.Date;
            var diff = (7 + (normalized.DayOfWeek - DayOfWeek.Monday)) % 7;
            return normalized.AddDays(-diff);
        }

        private static DayOfWeek NormalizeWeekday(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? DayOfWeek.Sunday : dayOfWeek;
        }

        private static string GetWeekdayLabel(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Thứ 2",
                DayOfWeek.Tuesday => "Thứ 3",
                DayOfWeek.Wednesday => "Thứ 4",
                DayOfWeek.Thursday => "Thứ 5",
                DayOfWeek.Friday => "Thứ 6",
                DayOfWeek.Saturday => "Thứ 7",
                _ => "Chủ nhật"
            };
        }

        private static double CalculateGrowth(decimal current, decimal previous)
        {
            if (previous == 0)
            {
                return current > 0 ? 100 : 0;
            }

            return Math.Round((double)((current - previous) / previous * 100), 2);
        }

        private static double CalculateGrowth(int current, int previous)
        {
            if (previous == 0)
            {
                return current > 0 ? 100 : 0;
            }

            return Math.Round(((double)(current - previous) / previous) * 100, 2);
        }
    }
}
