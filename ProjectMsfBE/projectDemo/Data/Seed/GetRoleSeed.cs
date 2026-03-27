using EventTick.Model.Enum;
using EventTick.Model.Models;

namespace projectDemo.Data.Seed
{
    public  class GetRoleSeed
    {
        public static List<Role> GetRole() { 
            return new List<Role>() 
            { 
                new Role { Id = 1, RoleName = EnumRoleName.ADMIN.ToString(),CreatedDate=DateTime.UtcNow},
                new Role { Id = 2, RoleName = EnumRoleName.ORGANIZER.ToString(),CreatedDate=DateTime.UtcNow},
                new Role { Id = 3, RoleName = EnumRoleName.CUSTOMER.ToString(), CreatedDate = DateTime.UtcNow},
            }; 
        
        }
    }
}
