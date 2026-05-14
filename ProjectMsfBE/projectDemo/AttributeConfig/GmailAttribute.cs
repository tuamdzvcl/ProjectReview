using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace projectDemo.AttributeConfig
{
    public class GmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        
            var email = value as string;
            if(string.IsNullOrEmpty(email))
            {
                return ValidationResult.Success;
            }
            var regex = new Regex(@"^(?!\.)(?!.*\.\.)([a-zA-Z0-9\.]{6,30})(?<!\.)@gmail\.com$");
            if(!regex.IsMatch(email))
            {
                return new ValidationResult("Email không đúng địng dạng");
            }
            return ValidationResult.Success;
        }
    }
}
