using System.ComponentModel.DataAnnotations;
using EventTick.Model.Enum;
using projectDemo.Entity.Enum;

namespace projectDemo.DTO.Request
{
    public class CreateEventTicketTypeItemRequest
    {
        [Required(ErrorMessage ="Không được để trốnh")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Không được để trống")]
        [Range(0.01, int.MaxValue)]
        public decimal Price { get; set; }
        [Required(ErrorMessage ="Không được để trống")]
        public int SoldQuantity { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalQuantity { get; set; }
        public EnumStatusTickType Status { get; set; } = EnumStatusTickType.ACTIVE;
    }
}
