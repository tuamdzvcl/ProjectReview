using EventTick.Model.Enum;
using projectDemo.AttributeConfig;
using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class EventRequest
    {

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]

        public string Title { get; set; }
        [MaxLength(ConfigValidation.MaxLengthDes,ErrorMessage ="Không được vướt quá {1} kí tự")]        
        public string? Description { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthStatus,ErrorMessage ="quá số kí tự rồi")]
        public string Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }
        [Required(ErrorMessage ="Đừng để trống nhé")]
        [MaxLength(100,ErrorMessage ="ĐỪng vượt quá nhé")]
        public string CatetoryName { get; set; }

        public List<CreateEventTicketTypeItemRequest> TicketTypes { get; set; }
        public IFormFile? PosterUrl { get; set; }
    }
}
