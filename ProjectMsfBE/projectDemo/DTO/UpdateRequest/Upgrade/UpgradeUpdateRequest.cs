using System;
using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.UpdateRequest.Upgrade
{
    public class UpgradeUpdateRequest
    {
        [Required]
        public string TitleUpgrade { get; set; }

        public string Description { get; set; }

        public string status { get; set; }

        public int DailyLimit { get; set; }

        public decimal Price { get; set; }
    }
}
