namespace projectDemo.DTO.Request
{
    public class RolePermissionResquest
    {
        public string RoleName { get; set; }

        public List<PermissionResquest> permissionResquests { get; set; }
    }
}
