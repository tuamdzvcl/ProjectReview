namespace projectDemo.DTO.Request
{
    public class TickCreateQrCode
    {
        public Guid OrderId { get; set; }
        public Guid EventID { get; set; }
        public int TickTypeId {  get; set; }

        
        public DateTime? expiration { get; set;}
    }
}
