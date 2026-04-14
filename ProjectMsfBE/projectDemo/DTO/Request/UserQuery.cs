namespace projectDemo.DTO.Request
{
    public class UserQuery
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? Keyword { get; set; }
        public string? Role { get; set; }
    }
}
