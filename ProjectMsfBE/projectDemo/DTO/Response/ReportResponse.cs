namespace projectDemo.DTO.Response
{
    public class ReportResponse
    {
        public RevenueSummaryDto Summary { get; set; } = new();
        public List<RevenueChartDto> Chart { get; set; } = new();
        public RevenueMetaDto Meta { get; set; } = new();
    }

    public class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalTickets { get; set; }
        public int TotalViews { get; set; }
        public double GrowthRevenue { get; set; }
        public double GrowthOrders { get; set; }
        public double GrowthTickets { get; set; }
        public double GrowthViews { get; set; }
    }

    public class RevenueChartDto
    {
        public DateTime Time { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class RevenueMetaDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GroupBy { get; set; } = string.Empty;
    }
}
