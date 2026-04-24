using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request
{
    public class LoginRequest
    {
        [EmailAddress]
        public string email { get; set; }

        public string password { get; set; }

       
    }
}
