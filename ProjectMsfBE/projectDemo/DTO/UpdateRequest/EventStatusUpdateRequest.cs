using EventTick.Model.Enum;
using projectDemo.Common;

namespace projectDemo.DTO.UpdateRequest
{
    public class EventStatusUpdateRequest
    {
        public string? Status { get; set; }
        public string? Reason { get; set; }
    }
}
