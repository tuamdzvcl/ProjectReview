namespace projectDemo.DTO.Request
{
    public class OrderDetailRequest
    {
        public Guid OrderDetailId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal SubTotal { get; set; }

        public string TicketTypeName { get; set; }
    }
}
