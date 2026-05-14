using projectDemo.AttributeConfig;
using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class ResetPasswordRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        [EmailAddress(ErrorMessage = "Email Không đúng định dạng")]
        public string Email { get; set; } = null!;

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthHashcode)]
        public string Token { get; set; } = null!;

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string NewPassword { get; set; } = null!;
    }
}
