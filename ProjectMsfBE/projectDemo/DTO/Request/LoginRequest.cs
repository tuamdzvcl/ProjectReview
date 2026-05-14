using projectDemo.AttributeConfig;
using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class LoginRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        [EmailAddress ]
        public string email { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string password { get; set; }

       
    }
}
