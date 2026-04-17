using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.Request.Upgrade
{
    public class UpgradeCreateRequest
    {
        [Required]
        public string TitleUpgrade { get; set; }

        public string Description { get; set; }

        public string status { get; set; }

        public int DailyLimit { get; set; }

        public decimal Price { get; set; }
    }
}
