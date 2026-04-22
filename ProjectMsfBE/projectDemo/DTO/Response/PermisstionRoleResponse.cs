namespace projectDemo.DTO.Response
{
    public class PermisstionRoleResponse
    {
        public int Id { get; set; }
        public string RoleName { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool IsSystem { get; set; }

        public List<PermissionResponse> Permissions { get; set; }
    }
}
