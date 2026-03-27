using EventTick.Model.Enum;

namespace projectDemo.DTO.Response
{
    public class RoleResponse
    {
        public string RoleName { get; set; } = null;
        public List<PermissionResponse> permissions { get; set; } = new List<PermissionResponse>();

    }
}
