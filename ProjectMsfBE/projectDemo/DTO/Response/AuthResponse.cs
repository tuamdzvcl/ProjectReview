namespace projectDemo.DTO.Response
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool Isnew { get; set; }

        public UserResponse User { get; set; }
    }
}
