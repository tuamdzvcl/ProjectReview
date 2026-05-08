using projectDemo.Common;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string FirstName { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string LastName { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        [EmailAddress(ErrorMessage ="Không đúng định dạng của gmail")]
        public string Email { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password không được chứa khoảng trắng")]
        public string password { get; set; }
    }
}
