using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class ForgotPasswordRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]

        [EmailAddress(ErrorMessage ="email không đúng định dạng")]
        public string Email { get; set; } = null!;
    }
}
