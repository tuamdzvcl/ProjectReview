using EventTick.Model.Enum;

namespace projectDemo.DTO.Response
{
    public class UserResponse
    {

        public string Email { get; set;}
        public string Username { get; set; }
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public List<string> RoleName { get; set; }

        public Guid ID { get; set;}
        public string? AvatarUrl { get; set; }

        public DateTime? DateLock { get; set; }
        public int? Isfalse { get; set; }
    }
}
