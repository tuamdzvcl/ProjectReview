using projectDemo.AttributeConfig;
using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class PermisstionRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string PermissonsName { get; set; }

        public string PermissonsDescription { get; set; }
    }
}
