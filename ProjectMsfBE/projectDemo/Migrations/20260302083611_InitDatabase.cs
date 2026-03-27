using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "PermissonsDescription", "PermissonsName" },
                values: new object[,]
                {
                    { 1, "tạo mới user", "user.create" },
                    { 2, "sửa user", "user.update" },
                    { 3, "xóa user", "user.delete" },
                    { 4, "xem user", "user.view" },
                    { 5, "tạo role mới", "role.create" },
                    { 6, " sửa role", "role.update" },
                    { 7, " xóa role", "role.delete" },
                    { 8, " xem role", "role.view" },
                    { 9, "tạo mới event", "event.create" },
                    { 10, "sửa  event", "event.update" },
                    { 11, "xóa  event", "event.delete" },
                    { 12, " xem event", "event.view" },
                    { 13, "xem tổng vé của event", "event.getTotalTickbyid" },
                    { 14, "xem tổng vé theo user", "event.getTotalTickByUser" }
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 3, 2, 8, 36, 11, 184, DateTimeKind.Utc).AddTicks(4840), new DateTime(2026, 3, 2, 8, 36, 11, 184, DateTimeKind.Utc).AddTicks(4847) });

            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 3, 2, 8, 36, 11, 184, DateTimeKind.Utc).AddTicks(4865), "$2a$11$9GrBLBFpueYoXyxaq1ZEduAhBP.bPIPqv9GHJKcJa2/pYvgApki1O", new DateTime(2026, 3, 2, 8, 36, 11, 184, DateTimeKind.Utc).AddTicks(4866) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 27, 10, 14, 31, 615, DateTimeKind.Utc).AddTicks(858), new DateTime(2026, 2, 27, 10, 14, 31, 615, DateTimeKind.Utc).AddTicks(861) });

            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 27, 10, 14, 31, 615, DateTimeKind.Utc).AddTicks(881), "$2a$11$Jn0fWRMJ/zN9WytFI8IFH.cG/u/0idlRKy5l9s42tM/bmWWeqZ/9q", new DateTime(2026, 2, 27, 10, 14, 31, 615, DateTimeKind.Utc).AddTicks(881) });
        }
    }
}
