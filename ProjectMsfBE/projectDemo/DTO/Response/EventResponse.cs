using EventTick.Model.Enum;
using System.Text.Json.Serialization;

namespace projectDemo.DTO.Response
{
    public class EventResponse
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
            public Guid? UserID { get; set; } = null;
        
    }
}
