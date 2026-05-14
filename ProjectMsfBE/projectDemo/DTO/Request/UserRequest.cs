using System.ComponentModel.DataAnnotations;
using EventTick.Model.Enum;
using projectDemo.AttributeConfig;
using projectDemo.Common;

namespace projectDemo.DTO.Request
{
    public class UserRequest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        [Gmail]
        public string Email { get; set; }

        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(
            @"^(?=.*[a-zA-ZÀ-ỹ])[a-zA-ZÀ-ỹ\s]+$",
            ErrorMessage = "Tên không hợp lệ (không chứa ký tự đặc biệt, không được chỉ có khoảng trắng)"
        )]
        [MaxLength(20,ErrorMessage ="Vượt quá kí tự cho phép")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(
            @"^(?=.*[a-zA-ZÀ-ỹ])[a-zA-ZÀ-ỹ\s]+$",
            ErrorMessage = "Tên không hợp lệ (không chứa ký tự đặc biệt, không được chỉ có khoảng trắng)"
        )]
        [MaxLength(20, ErrorMessage = "Vượt quá kí tự cho phép")]
        public string LastName { get; set; }
        public string AvataUrl { get; set; }
        public List<int> RoleName { get; set; }
    }
}
