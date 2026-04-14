using projectDemo.DTO.Response;

namespace projectDemo.DTO.Request
{
    public class CreateOrderRequest
    {
        public UserOrder User { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }
}
