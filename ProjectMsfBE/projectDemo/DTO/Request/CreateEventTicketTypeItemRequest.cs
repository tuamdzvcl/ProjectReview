using EventTick.Model.Enum;
using projectDemo.Entity.Enum;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class CreateEventTicketTypeItemRequest
    {
        [Required]
        public EnumNameTickType Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int SoldQuantity { get; set; } = 0;

        [Required]
        public EnumStatusTickType Status { get; set; } = EnumStatusTickType.ACTIVE;
    }
}
