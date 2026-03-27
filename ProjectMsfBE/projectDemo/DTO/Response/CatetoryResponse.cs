namespace projectDemo.DTO.Response
{
    public class CatetoryResponse
    {
        public Guid CatetoryId { get; set; }
        public string Name { get; set; }
        
        public List<EventResponse> listEvent { get; set; }  

    }
}
