using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace projectDemo.Data.InitDB
{
    public class SeedConstants
    {
        public static readonly Guid AdminUserId =
       Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static readonly Guid AdminUserLoginId =
            Guid.Parse("22222222-2222-2222-2222-222222222222");

    }
}
