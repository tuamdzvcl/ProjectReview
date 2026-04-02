using EventTick.Model.Enum;
using projectDemo.Entity.Enum;

namespace projectDemo.DTO.UpdateRequest
{
    public class UpdateEventTicketTypeItemRequest
    {
        public int? Id { get; set; }
        public EnumNameTickType? Name { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? Price { get; set; }
        public EnumStatusTickType? Status { get; set; }
    }
}
