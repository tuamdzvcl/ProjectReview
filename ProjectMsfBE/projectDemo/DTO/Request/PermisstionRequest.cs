using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class PermisstionRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string PermissonsName { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]

        public string PermissonsDescription { get; set; }
    }
}
