namespace projectDemo.DTO.Response
{
    public class UserEventsResponse
    {
        public UserResponse User { get; set; } = null!;
        public List<EventResponse> Events { get; set; } = new();
    }
}
