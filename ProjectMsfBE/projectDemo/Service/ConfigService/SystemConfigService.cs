using DocumentFormat.OpenXml.Spreadsheet;
using projectDemo.Common;

namespace projectDemo.Service.ConfigService
{
    public class SystemConfigService : ISystemConfigService 
    {
        public object GetSystemConfig() {
            return new {
                Validation = new {
                    Default = new { Min = ConfigValidation.MinLength, Max = ConfigValidation.MaxLength },
                    DefaultNumber = new {Min= 1000,Max= 9999999999 },   
                    User = new {
                        FirstName = new { Min = ConfigValidation.FirstNameMinLength, Max = ConfigValidation.FirstNameMaxLength },
                        LastName = new { Min = ConfigValidation.LastNameMinLength, Max = ConfigValidation.LastNameMaxLength },
                        Email = new { Max = ConfigValidation.EmailMaxLength },
                        Password = new { Min = ConfigValidation.MinLength, Max = ConfigValidation.MaxLength, Regex = ConfigValidation.PasswordRegex }
                    },
                    Event = new {
                        Title = new { Min = ConfigValidation.MinLength, Max = ConfigValidation.MaxLength },
                        Description = new { Min = ConfigValidation.MinLengthDes, Max = ConfigValidation.MaxLengthDes },
                        Category = new { Min = ConfigValidation.CatetoryMinLength, Max = ConfigValidation.CatetoryMaxLength }
                    },
                    TickType = new{
                        Name = new { Min = ConfigValidation.MinLength, Max = ConfigValidation.MaxLength },
                    }
                   

                },
                Pagination = new {
                    DefaultPageSize = ConfigValidation.DefaultPageSize,
                    MaxPageSize = ConfigValidation.MaxPageSize
                }
            };
        }
    }
}
