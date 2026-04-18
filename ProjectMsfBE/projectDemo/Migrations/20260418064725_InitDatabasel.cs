using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabasel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackGroupUrl",
                table: "User",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 18, 6, 47, 25, 513, DateTimeKind.Utc).AddTicks(3451));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 18, 6, 47, 25, 513, DateTimeKind.Utc).AddTicks(3472));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 18, 6, 47, 25, 513, DateTimeKind.Utc).AddTicks(3474));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "BackGroupUrl", "PasswordHash" },
                values: new object[] { null, "$2a$11$kZnHVxPlXOt0cuJNS8XBROkEN.9m2mbm6k87NYJ8AGLyGXyn1Bbqy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackGroupUrl",
                table: "User");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 18, 3, 34, 6, 357, DateTimeKind.Utc).AddTicks(5272));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 18, 3, 34, 6, 357, DateTimeKind.Utc).AddTicks(5294));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 18, 3, 34, 6, 357, DateTimeKind.Utc).AddTicks(5297));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$IW102BvZINE3fqmoMRFBduP44JrjPvKE6mkJwLzqsl9hGZ2wSZqyC");
        }
    }
}
