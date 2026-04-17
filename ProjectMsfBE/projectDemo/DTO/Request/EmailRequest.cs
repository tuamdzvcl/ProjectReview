namespace projectDemo.DTO.Request
{
    public class EmailRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string phone { get; set; }

        public Guid OrderId { get; set; }

    }
}
