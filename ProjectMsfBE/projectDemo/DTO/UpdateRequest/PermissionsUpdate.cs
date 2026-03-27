using System.ComponentModel.DataAnnotations;

namespace projectDemo.DTO.UpdateRequest
{
    public class PermissionsUpdate
    {
        public string PermissonsName { get; set; }
        
        public string PermissonsDescription { get; set; }
    }
}