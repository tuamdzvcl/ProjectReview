using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class CreateEventWithTicketTypesRequest : EventRequest
    {
        [Required]
        [MinLength(1)]
        public List<CreateEventTicketTypeItemRequest> TicketTypes { get; set; } = new();
    }
}
