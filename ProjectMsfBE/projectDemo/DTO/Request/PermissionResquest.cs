using projectDemo.Common;

namespace projectDemo.DTO.Request
{
    public class PermissionResquest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public int PermissionId { get; set; }

    }
}
