using EventTick.Model.Enum;
using projectDemo.AttributeConfig;
using projectDemo.Common;

namespace projectDemo.DTO.UpdateRequest
{
    public class EventUpdateRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]

        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public IFormFile? PosterUrl { get; set; }
        public string? Status { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string? CatetoryName { get; set; }
        public List<UpdateEventTicketTypeItemRequest>? TicketTypes { get; set; }
    }
}
