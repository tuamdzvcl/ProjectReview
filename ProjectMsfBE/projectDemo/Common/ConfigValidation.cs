namespace projectDemo.Common
{
    public static class ConfigValidation
    {
        public const int MaxLength = 255;
        public const int MinLength = 6;
        public const int FirstNameMaxLength = 20;
        public const int FirstNameMinLength = 3;
        public const int LastNameMaxLength = 20;
        public const int LastNameMinLength = 3;
        public const int EmailMaxLength = 50;
        public const int MaxLengthStatus = 50;
        public const int MaxLengthHashcode = 500;
        public const int MaxLengthDes = 2000;
        public const int MinLengthDes = 20;
        public const int CatetoryMaxLength = 50;
        public const int CatetoryMinLength = 2;

        // Pagination
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        // Regex patterns
        public const string PasswordRegex = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$";
        public const string PhoneRegex = @"^(0[3|5|7|8|9])+([0-9]{8})$";


    }
}
