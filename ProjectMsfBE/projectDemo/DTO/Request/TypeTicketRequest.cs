using EventTick.Model.Enum;
using projectDemo.Entity.Enum;

namespace projectDemo.DTO.Request
{
    public class TypeTicketRequest
    {
        public EnumNameTickType Name { get; set; }
        public int TotalQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public EnumStatusTickType Status { get; set; }
        public decimal Price { get; set; }

        public Guid EventID { get; set; }
    }
}
