using projectDemo.Common;

namespace projectDemo.DTO.Request
{
    public class RolePermissionResquest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]

        public string RoleName { get; set; }

        public List<PermissionResquest> permissionResquests { get; set; }
    }
}
