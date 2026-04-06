namespace projectDemo.DTO.Query
{
    public class OrderEventFlatRow
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TicketPrice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Status { get; set; }
        public Guid? EventId { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public string? EventLocation { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string? EventPosterUrl { get; set; }
        public Guid? OrderDetailId { get; set; }
        public int? TicketTypeId { get; set; }
        public string? TicketTypeName { get; set; }
        public int? TicketQuantity { get; set; }
    }
}

