using EventTick.Model.Enum;
using EventTick.Model.Models;
using projectDemo.DTO.Response;

namespace projectDemo.DTO.Projection
{
    public class EventProjection
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public string PosterUrl { get; set; }
        public EnumStatusEvent Status { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }

        public List<TicketType> listTypeTick {  get; set; }
    }
}
