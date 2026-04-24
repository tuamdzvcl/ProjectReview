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
                    PermissonsName = "user_view_all".ToUpper(),
                    PermissonsDescription = "xem tất cả user sự dụng hệ thống",
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
                    PermissonsName = "event_view_detial".ToUpper(),
                    PermissonsDescription = " xem chi tiết thông tin event",
                },
                new Permissions
                {
                    Id = 13,
                    PermissonsName = "View_Upgrade".ToUpper(),
                    PermissonsDescription = "xem vé thành viên",
                },
                new Permissions
                {
                    Id = 14,
                    PermissonsName = "Create_Upgrade".ToUpper(),
                    PermissonsDescription = "tạo vé thành viên",
                },
                //Admin
                new Permissions
                {
                    Id = 15,
                    PermissonsName = "Veiw_dashboard".ToUpper(),
                    PermissonsDescription = "xem báo cáo doanh thu",
                },
                new Permissions
                {
                    Id = 16,
                    PermissonsName = "View_dashboarhAll".ToUpper(),
                    PermissonsDescription = "xem báo cáo doanh thu của hệ thống",
                },
                new Permissions
                {
                    Id = 17,
                    PermissonsName = "View_KM".ToUpper(),
                    PermissonsDescription = "xem khuyễn mãi",
                },
                new Permissions
                {
                    Id = 18,
                    PermissonsName = "View_Audilog".ToUpper(),
                    PermissonsDescription = " xem log của hệ thống",
                },
                new Permissions
                {
                    Id = 19,
                    PermissonsName = "Upgrade_delete".ToUpper(),
                    PermissonsDescription = "xóa vé thành viên",
                },
                new Permissions
                {
                    Id = 20,
                    PermissonsName = "Event_browse".ToUpper(),
                    PermissonsDescription = "Duyệt event",
                },
                new Permissions
                {
                    Id = 21,
                    PermissonsName = "User_view_organisation".ToUpper(),
                    PermissonsDescription = "Xem các user đã tham gia sự kiện",
                },
                new Permissions
                {
                    Id = 22,
                    PermissonsName = "Edit_crad".ToUpper(),
                    PermissonsDescription = "sửa thông tin thẻ",
                },
                new Permissions
                {
                    Id = 23,
                    PermissonsName = "View_Payment".ToUpper(),
                    PermissonsDescription = "xem danh sách thanh toán",
                },
            };
        }
    }
}
