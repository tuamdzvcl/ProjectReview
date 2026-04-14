using EventTick.Model.Enum;

namespace projectDemo.DTO.Request
{
    public class UserRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvataUrl { get; set; }
        public List<string> RoleName { get; set; }
    }
}
