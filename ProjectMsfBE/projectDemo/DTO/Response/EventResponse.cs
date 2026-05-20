using System.Text.Json.Serialization;
using EventTick.Model.Enum;

namespace projectDemo.DTO.Response
{
    public class EventResponse
    {
        public Guid EventID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Location { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset? SaleStartDate { get; set; }
        public DateTimeOffset? SaleEndDate { get; set; }
        public string PosterUrl { get; set; }
        public string Status { get; set; }
        public Guid? UserID { get; set; } = null;
    }
}
