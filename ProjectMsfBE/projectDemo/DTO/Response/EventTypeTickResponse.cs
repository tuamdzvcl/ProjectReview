namespace projectDemo.DTO.Response
{
    public class EventTypeTickResponses
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public string PosterUrl { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public Guid CatetoryID { get; set; }
        public string CatetoryName { get; set; }
        public Guid UserID { get; set; }
        public List<TypeTickResponse> ListTypeTick { get; set; } = new();
    }
}
