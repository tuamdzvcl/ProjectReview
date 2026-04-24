using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabaseg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem tất cả user sự dụng hệ thống", "USER_VIEW_ALL" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { " xem chi tiết thông tin event", "EVENT_VIEW_DETIAL" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem vé thành viên", "VIEW_UPGRADE" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "tạo vé thành viên", "CREATE_UPGRADE" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem báo cáo doanh thu", "VEIW_DASHBOARD" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem báo cáo doanh thu của hệ thống", "VIEW_DASHBOARHALL" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem khuyễn mãi", "VIEW_KM" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { " xem log của hệ thống", "VIEW_AUDILOG" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xóa vé thành viên", "UPGRADE_DELETE" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsDeleted", "PermissonsDescription", "PermissonsName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 20, null, null, false, "Duyệt event", "EVENT_BROWSE", null, null },
                    { 21, null, null, false, "Xem các user đã tham gia sự kiện", "USER_VIEW_ORGANISATION", null, null },
                    { 22, null, null, false, "sửa thông tin thẻ", "EDIT_CRAD", null, null },
                    { 23, null, null, false, "xem danh sách thanh toán", "VIEW_PAYMENT", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 23, 14, 1, 43, 149, DateTimeKind.Utc).AddTicks(2218));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 23, 14, 1, 43, 149, DateTimeKind.Utc).AddTicks(2230));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 23, 14, 1, 43, 149, DateTimeKind.Utc).AddTicks(2233));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$1DjGMiHF6mMFZH0ZXcvjnuDWrGBAJSeTYbDUkKoUll6QNELTJZET6");

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "CreatedBy", "CreatedDate", "Id", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 20, 1, null, null, 20, false, null, null },
                    { 21, 1, null, null, 21, false, null, null },
                    { 22, 1, null, null, 22, false, null, null },
                    { 23, 1, null, null, 23, false, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 20, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 21, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 22, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 23, 1 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem user", "USER_VIEW" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { " xem event", "EVENT_VIEW" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem tổng vé của event", "EVENT_GETTOTALTICKBYID" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem tổng vé theo user", "EVENT_GETTOTALTICKBYUSER" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "tạo mới TypeTicket", "TYPETICKET_CREATE" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "sửa  TypeTicket", "TYPETICKET_UPDATE" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xóa  TypeTicket", "TYPETICKET_DELETE" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { " xem TypeTicket", "TYPETICKET_VIEW" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "PermissonsDescription", "PermissonsName" },
                values: new object[] { "xem tổng vé của TypeTicket", "TYPETICKET_GETROLEBYID" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 6, 39, 59, 371, DateTimeKind.Utc).AddTicks(168));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 6, 39, 59, 371, DateTimeKind.Utc).AddTicks(181));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 6, 39, 59, 371, DateTimeKind.Utc).AddTicks(183));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$YAEOOSZW/uj2lmwwKymwp.Cj74.ShzbCD6gMGe6LQUGXb88Swoquy");
        }
    }
}
