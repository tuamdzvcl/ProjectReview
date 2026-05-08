using projectDemo.Common;

namespace projectDemo.DTO.Response
{
    public class UserOrder
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]

        public string fullName { get; set; }
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string Email { get; set; }
    }
}
