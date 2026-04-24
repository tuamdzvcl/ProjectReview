using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabaseek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Isfalse",
                table: "Event",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 24, 7, 20, 28, 723, DateTimeKind.Utc).AddTicks(6624));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 24, 7, 20, 28, 723, DateTimeKind.Utc).AddTicks(6631));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 24, 7, 20, 28, 723, DateTimeKind.Utc).AddTicks(6633));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$Jjawr85fB4zt8nZ0nKO.6OF3ProsnIy4ZESvMVB9SFBXUzBJIV8g.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Isfalse",
                table: "Event");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 24, 5, 11, 58, 32, DateTimeKind.Utc).AddTicks(6281));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 24, 5, 11, 58, 32, DateTimeKind.Utc).AddTicks(6290));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 24, 5, 11, 58, 32, DateTimeKind.Utc).AddTicks(6293));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$W7R6fvif6F1zd5QwovqMa.eKNgJ265eC1Y.c5BZ45N5f9IMF0bJxa");
        }
    }
}
