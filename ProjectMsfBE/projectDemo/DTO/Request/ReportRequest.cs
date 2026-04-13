namespace projectDemo.DTO.Request
{
    public class ReportRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string GroupBy { get; set; }
    }
}
