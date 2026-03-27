using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class updatetablerole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Role",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "PermissonsName",
                value: "USER_CREATE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "PermissonsName",
                value: "USER_UPDATE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "PermissonsName",
                value: "USER_DELETE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "PermissonsName",
                value: "USER_VIEW");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "PermissonsName",
                value: "ROLE_CREATE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "PermissonsName",
                value: "ROLE_UPDATE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "PermissonsName",
                value: "ROLE_DELETE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "PermissonsName",
                value: "ROLE_VIEW");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "PermissonsName",
                value: "EVENT_CREATE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "PermissonsName",
                value: "EVENT_UPDATE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "PermissonsName",
                value: "EVENT_DELETE");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "PermissonsName",
                value: "EVENT_VIEW");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "PermissonsName",
                value: "EVENT_GETTOTALTICKBYID");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "PermissonsName",
                value: "EVENT_GETTOTALTICKBYUSER");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsDeleted", "PermissonsDescription", "PermissonsName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 15, null, null, false, "tạo mới TypeTicket", "TYPETICKET_CREATE", null, null },
                    { 16, null, null, false, "sửa  TypeTicket", "TYPETICKET_UPDATE", null, null },
                    { 17, null, null, false, "xóa  TypeTicket", "TYPETICKET_DELETE", null, null },
                    { 18, null, null, false, " xem TypeTicket", "TYPETICKET_VIEW", null, null },
                    { 19, null, null, false, "xem tổng vé của TypeTicket", "TYPETICKET_GETROLEBYID", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoleName",
                value: "ADMIN");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoleName",
                value: "ORGANIZER");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "RoleName",
                value: "CUSTOMER");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$OY5EoXBhlu1Z9.AQOxZU7OyE3eZK3DhJ7ErSAATeO3q80UdYN2UCy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.AlterColumn<int>(
                name: "RoleName",
                table: "Role",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "PermissonsName",
                value: "user.create");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "PermissonsName",
                value: "user.update");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "PermissonsName",
                value: "user.delete");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "PermissonsName",
                value: "user.view");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "PermissonsName",
                value: "role.create");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "PermissonsName",
                value: "role.update");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "PermissonsName",
                value: "role.delete");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "PermissonsName",
                value: "role.view");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "PermissonsName",
                value: "event.create");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "PermissonsName",
                value: "event.update");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "PermissonsName",
                value: "event.delete");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "PermissonsName",
                value: "event.view");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "PermissonsName",
                value: "event.getTotalTickbyid");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "PermissonsName",
                value: "event.getTotalTickByUser");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoleName",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoleName",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "RoleName",
                value: 3);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$e0hkFSeLfG0vaqDOeybMROqR4KAc1LufJubyP9mZrqcIsyWFvkEd2");
        }
    }
}
