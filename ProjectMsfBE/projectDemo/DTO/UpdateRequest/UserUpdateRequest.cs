using EventTick.Model.Enum;

namespace projectDemo.DTO.UpdateRequest
{
    public class UserUpdateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string AvataUrl { get; set; }

        public List<int> RoleName { get; set; }

        public DateTime UpdateAt { get; set; }

        public string UpdateBy { get; set; }
    }
}
