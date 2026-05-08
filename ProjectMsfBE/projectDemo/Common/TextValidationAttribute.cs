using System.ComponentModel.DataAnnotations;

namespace projectDemo.Common
{
    public class TextValidationAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public TextValidationAttribute(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var str = value as string;

            if (string.IsNullOrWhiteSpace(str))
                return new ValidationResult("Không được để trống");

            if (str.Length < _minLength)
                return new ValidationResult($"Không được ít hơn {_minLength} kí tự");

            if (str.Length > _maxLength)
                return new ValidationResult($"Không vượt quá {_maxLength} kí tự");

            return ValidationResult.Success;
        }
    }
}
