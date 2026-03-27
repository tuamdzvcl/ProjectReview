using EventTick.Model.Enum;
using projectDemo.DTO.Response;

namespace projectDemo.DTO.Projection
{
    public class OrderProjection
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public EnumStatusOrder Status { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<OrderDetialProjection> orderDetails { get; set; }
    }
}
