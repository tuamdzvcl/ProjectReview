namespace projectDemo.DTO.Response
{
    public class CatetoryEventRaw
    {
        public Guid CatetoryId { get; set; }
        public string CatetoryName { get; set; }

        public Guid EventID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string PosterUrl { get; set; }
        public int EventStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
