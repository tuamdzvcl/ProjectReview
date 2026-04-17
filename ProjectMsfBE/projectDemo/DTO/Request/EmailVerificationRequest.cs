using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; } = null!;
    }

    public class ResendVerificationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
