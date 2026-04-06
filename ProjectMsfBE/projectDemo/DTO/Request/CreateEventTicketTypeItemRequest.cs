using EventTick.Model.Enum;
using projectDemo.Entity.Enum;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class CreateEventTicketTypeItemRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public int SoldQuantity { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalQuantity { get; set; }
        public EnumStatusTickType Status { get; set; } = EnumStatusTickType.ACTIVE;
    }
}
