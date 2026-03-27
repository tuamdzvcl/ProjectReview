using EventTick.Model.Enum;
using projectDemo.Entity.Enum;

namespace projectDemo.DTO.Projection
{
    public class TypeTickProjection
    {
        public int Id { get; set; }
        public EnumNameTickType Name { get; set; }
        public int TotalQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public EnumStatusTickType Status { get; set; }
    }
}
