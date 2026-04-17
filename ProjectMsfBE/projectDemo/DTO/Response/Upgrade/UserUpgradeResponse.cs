using System;

namespace projectDemo.DTO.Response.Upgrade
{
    public class UserUpgradeResponse
    {
        public Guid Id { get; set; }
        public string UpgradeTitle { get; set; }
        public int DailyLimit { get; set; }
        public int CurrentDayUsageCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public decimal PricePaid { get; set; }
        public bool IsDailyPackage { get; set; }
    }
}
