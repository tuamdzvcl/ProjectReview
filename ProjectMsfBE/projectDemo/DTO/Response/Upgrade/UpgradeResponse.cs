using System;

namespace projectDemo.DTO.Response.Upgrade
{
    public class UpgradeResponse
    {
        public int Id { get; set; }

        public string TitleUpgrade { get; set; }

        public string Description { get; set; }

        public string status { get; set; }

        public int DailyLimit { get; set; }

        public decimal Price { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
