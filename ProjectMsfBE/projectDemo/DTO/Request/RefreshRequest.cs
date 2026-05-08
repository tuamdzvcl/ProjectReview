using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class RefreshRequest
    {
        [Required(ErrorMessage = "Refresh token không được để trống")]
        public string RefreshToken { get; set; }
    }
}
