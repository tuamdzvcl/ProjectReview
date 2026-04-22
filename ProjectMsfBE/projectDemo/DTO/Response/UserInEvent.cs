using System.Reflection;

namespace projectDemo.DTO.Response
{
    public class UserInEvent
    {
        public string Email { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid ID { get; set; }
        public string Avarta { get; set; }
        public List<EventInfo> Events { get; set; }


    }
    public class TicketInfo
    {
        public string? TicketName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
    public class EventInfo
    {
        public string EventTitle { get; set; }
        public List<TicketInfo> Tickets { get; set; }
    }
}
