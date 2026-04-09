namespace projectDemo.DTO.Query
{
    public class RevenueReportFlatRow
    {
        public Guid OrderId { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal Revenue { get; set; }
        public int TicketQuantity { get; set; }
    }
}
