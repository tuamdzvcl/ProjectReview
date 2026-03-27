namespace projectDemo.Common.PageRequest
{
    public class PageRequest
    {
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;
        public string? key { get; set; } = null;
    }
}
