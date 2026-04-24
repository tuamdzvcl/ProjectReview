using EventTick.Model.Enum;

namespace projectDemo.DTO.UpdateRequest
{
    public class EventStatusUpdateRequest
    {
        public EnumStatusEvent? Status { get; set; }
        public string? Reason { get; set; }
    }
}
