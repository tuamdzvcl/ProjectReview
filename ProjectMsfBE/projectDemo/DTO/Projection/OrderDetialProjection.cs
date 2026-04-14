namespace projectDemo.DTO.Projection
{
    public class OrderDetialProjection
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int TicketTypeName { get; set; }
    }
}
