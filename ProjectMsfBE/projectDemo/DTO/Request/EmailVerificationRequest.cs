using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class VerifyEmailRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthHashcode)]

        public string Token { get; set; } = null!;
    }

    public class ResendVerificationRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        [EmailAddress(ErrorMessage ="Không đúng định dạng email")]
        public string Email { get; set; } = null!;
    }
}
