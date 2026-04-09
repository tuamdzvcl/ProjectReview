namespace projectDemo.DTO.Response
{
    public class CreateEventWithTicketTypesResponse
    {
        public Guid EventID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public string PosterUrl { get; set; }
        public string Status { get; set; }
        public Guid? UserID { get; set; }
        public string CatetoryName { get; set; }
        public List<TypeTickResponse> TicketTypes { get; set; } = new();
    }
}
