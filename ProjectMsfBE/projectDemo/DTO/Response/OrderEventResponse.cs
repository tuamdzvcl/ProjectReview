namespace projectDemo.DTO.Response
{
    public class OrderEventResponse
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Status { get; set; }
        public EventOrder? Event { get; set; }
    }
}

public class EventOrder
{
    public Guid EventID { get; set;  }
    public string EventName { get; set; } = string.Empty;
    public string EventDescription { get; set; }= string.Empty;
    public string EventLocation { get; set;  }
    public DateTime? EventStartDate { get; set; }

    public DateTime? EventEndDate { get; set; }

    public string EventPosterUrl { get; set;}

    public List<TypeTickOrder> ListTypeTicket { get; set; }
}
public class TypeTickOrder
{
    public Guid OrderDetailId { get; set; }
    public int TicketTypeId { get; set; } = 0;
    public string TicketTypeName { get; set; } = null;
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set;}
}
