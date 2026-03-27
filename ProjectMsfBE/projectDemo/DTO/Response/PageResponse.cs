namespace projectDemo.DTO.Respone
{
    public class PageResponse<T> : ApiResponse<T>
    {
        public List<T>? Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}
