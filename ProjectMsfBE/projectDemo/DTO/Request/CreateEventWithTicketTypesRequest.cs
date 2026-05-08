using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class CreateEventWithTicketTypesRequest : EventRequest
    {
       
        public List<CreateEventTicketTypeItemRequest> TicketTypes { get; set; } = new();
    }
}
