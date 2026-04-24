using System;

namespace projectDemo.DTO.Response
{
    public class ParticipantSummaryResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Avarta { get; set; }
        public int EventCount { get; set; }
    }
}
