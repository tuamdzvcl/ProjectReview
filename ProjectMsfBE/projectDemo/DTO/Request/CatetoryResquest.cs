using projectDemo.Common;

namespace projectDemo.DTO.Request
{
    public class CatetoryResquest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLength)]
        public string Name { get; set; }
    }
}
