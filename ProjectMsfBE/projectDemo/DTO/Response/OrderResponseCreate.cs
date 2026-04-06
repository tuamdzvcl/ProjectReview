namespace projectDemo.DTO.Response
{
    public class OrderResponseCreate
    {
        

        public Guid OrderId { get; set; }

        public decimal TotalAmount { get; set; }

        public List<OrderDetailResponse> Items { get; set; }
    }
}
