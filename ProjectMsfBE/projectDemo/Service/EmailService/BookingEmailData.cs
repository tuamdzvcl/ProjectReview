namespace projectDemo.Service.EmailService
{
    public class BookingEmailData
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public string EventName { get; set; } = string.Empty;
        public string EventLocation { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventPosterUrl { get; set; } = string.Empty;

        public List<TicketEmailItem> Tickets { get; set; } = new();
    }

    public class TicketEmailItem
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string QRCodeUrl { get; set; } = string.Empty;
    }
}
