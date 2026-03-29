using EventTick.Model.Enum;

namespace projectDemo.DTO.Request
{
    public class EventRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public string CatetoryName { get; set; }

        public List<TypeTicketRequest> typeTicketRequests { get; set; }
        public IFormFile? PosterUrl { get; set; }
    }
}
