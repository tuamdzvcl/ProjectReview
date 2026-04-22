using System;

namespace projectDemo.DTO.Query
{
    public class ParticipantFlatRow
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? EventTitle { get; set; }
        public string? TicketName { get; set; }
        public decimal? TotalAmount { get; set; }
        public int TicketQuantity { get; set; }
        public DateTime? DateLock { get; set; }
        public int? Isfalse { get; set; }
    }
}
