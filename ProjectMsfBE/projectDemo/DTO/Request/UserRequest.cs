using System.ComponentModel.DataAnnotations;
using EventTick.Model.Enum;

namespace projectDemo.DTO.Request
{
    public class UserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(
            @"^(?=.*[a-zA-ZÀ-ỹ])[a-zA-ZÀ-ỹ\s]+$",
            ErrorMessage = "Tên không hợp lệ (không chứa ký tự đặc biệt, không được chỉ có khoảng trắng)"
        )]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(
            @"^(?=.*[a-zA-ZÀ-ỹ])[a-zA-ZÀ-ỹ\s]+$",
            ErrorMessage = "Tên không hợp lệ (không chứa ký tự đặc biệt, không được chỉ có khoảng trắng)"
        )]
        public string LastName { get; set; }
        public string AvataUrl { get; set; }
        public List<int> RoleName { get; set; }
    }
}
