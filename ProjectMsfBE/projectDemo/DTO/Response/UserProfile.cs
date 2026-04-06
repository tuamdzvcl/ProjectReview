namespace projectDemo.DTO.Response
{
    public class UserProfile
    {
        public UserResponseProfile User { get; set; }

        public List<EventResponse> Events {  get; set; }
    }

}
public class UserResponseProfile
{

    public string LastName { get; set; }

    public string FirstName { get; set; }

    public string? AvatarUrl { get; set; }
    public List<string> RoleName { get; set; }

}
