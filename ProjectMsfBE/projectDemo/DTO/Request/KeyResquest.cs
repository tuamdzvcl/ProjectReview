using projectDemo.AttributeConfig;
using projectDemo.Common;

namespace projectDemo.DTO.Request
{
    public class KeyResquest
    {
        [TextValidation(ConfigValidation.MinLength, ConfigValidation.MaxLengthHashcode)]
        public string key { get; set; }
    }
}
