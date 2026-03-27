using EventTick.Model.Enum;

namespace projectDemo.DTO.Projection
{
    public class RoleProjecttion
    {
        public string RoleName { get; set; }
        public List<PermissionProjection> ListPermissions { get; set; }
    }
}
