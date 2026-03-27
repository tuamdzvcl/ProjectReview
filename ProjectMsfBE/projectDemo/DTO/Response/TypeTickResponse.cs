using EventTick.Model.Enum;
using projectDemo.Entity.Enum;

namespace projectDemo.DTO.Response
{
    public class TypeTickResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int TotalQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public string  Status { get; set; }
    }
}
