using EventTick.Model.Models;
using projectDemo.Entity.Models;

namespace projectDemo.Data.Seed
{
    public class GetPermissionSeed
    {
        public static List<Permissions> GetPermission()
        {
            return new List<Permissions>
            {
                //User
                new Permissions
                {
                    Id = 1,
                    PermissonsName = "user_create".ToUpper(),
                    PermissonsDescription = "tạo mới user",
                },
                new Permissions
                {
                    Id = 2,
                    PermissonsName = "user_update".ToUpper(),
                    PermissonsDescription = "sửa user",
                },
                new Permissions
                {
                    Id = 3,
                    PermissonsName = "user_delete".ToUpper(),
                    PermissonsDescription = "xóa user",
                },
                new Permissions
                {
                    Id = 4,
                    PermissonsName = "user_view".ToUpper(),
                    PermissonsDescription = "xem user",
                },
                //Role
                new Permissions
                {
                    Id = 5,
                    PermissonsName = "role_create".ToUpper(),
                    PermissonsDescription = "tạo role mới",
                },
                new Permissions
                {
                    Id = 6,
                    PermissonsName = "role_update".ToUpper(),
                    PermissonsDescription = " sửa role",
                },
                new Permissions
                {
                    Id = 7,
                    PermissonsName = "role_delete".ToUpper(),
                    PermissonsDescription = " xóa role",
                },
                new Permissions
                {
                    Id = 8,
                    PermissonsName = "role_view".ToUpper(),
                    PermissonsDescription = " xem role",
                },
                //event
                new Permissions
                {
                    Id = 9,
                    PermissonsName = "event_create".ToUpper(),
                    PermissonsDescription = "tạo mới event",
                },
                new Permissions
                {
                    Id = 10,
                    PermissonsName = "event_update".ToUpper(),
                    PermissonsDescription = "sửa  event",
                },
                new Permissions
                {
                    Id = 11,
                    PermissonsName = "event_delete".ToUpper(),
                    PermissonsDescription = "xóa  event",
                },
                new Permissions
                {
                    Id = 12,
                    PermissonsName = "event_view".ToUpper(),
                    PermissonsDescription = " xem event",
                },
                new Permissions
                {
                    Id = 13,
                    PermissonsName = "event_getTotalTickbyid".ToUpper(),
                    PermissonsDescription = "xem tổng vé của event",
                },
                new Permissions
                {
                    Id = 14,
                    PermissonsName = "event_getTotalTickByUser".ToUpper(),
                    PermissonsDescription = "xem tổng vé theo user",
                },
                //typeticket8
                new Permissions
                {
                    Id = 15,
                    PermissonsName = "TypeTicket_create".ToUpper(),
                    PermissonsDescription = "tạo mới TypeTicket",
                },
                new Permissions
                {
                    Id = 16,
                    PermissonsName = "TypeTicket_update".ToUpper(),
                    PermissonsDescription = "sửa  TypeTicket",
                },
                new Permissions
                {
                    Id = 17,
                    PermissonsName = "TypeTicket_delete".ToUpper(),
                    PermissonsDescription = "xóa  TypeTicket",
                },
                new Permissions
                {
                    Id = 18,
                    PermissonsName = "TypeTicket_view".ToUpper(),
                    PermissonsDescription = " xem TypeTicket",
                },
                new Permissions
                {
                    Id = 19,
                    PermissonsName = "TypeTicket_getrolebyid".ToUpper(),
                    PermissonsDescription = "xem tổng vé của TypeTicket",
                },
            };
        }
    }
}
