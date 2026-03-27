using projectDemo.DTO.Response;

namespace projectDemo.DTO.Request
{
    public class CreateOrderRequest
    {
        public List<OrderItemRequest> Items { get; set; }

    }
}
