using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class RegisterRequest
    {
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password phải ít nhất 6 ký tự")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password không được chứa khoảng trắng")]
        public string password { get; set; }
    }
}
