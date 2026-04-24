using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabasee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Event");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 23, 15, 49, 20, 513, DateTimeKind.Utc).AddTicks(3228));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 23, 15, 49, 20, 513, DateTimeKind.Utc).AddTicks(3240));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 23, 15, 49, 20, 513, DateTimeKind.Utc).AddTicks(3242));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$l6IRUAjxuLbQ6yAHW/SgBO/sFcgDw8RaYTHefF0jMM9q0sFpbDs5m");
        }
    }
}
