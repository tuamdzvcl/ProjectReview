using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class updateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateLock",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Isfalse",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "DateLock", "Isfalse" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$RbrANq5ASNElSPB01XpQUu6/Kz9b5KEogeOoug0oS0BSEWQGYAFWm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateLock",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Isfalse",
                table: "User");

            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$qbdc0.AwiiIxb/kQTQ3PiOhvbEBYRXfJpdjQJtUoZnz/3Tpa3Fkoa");
        }
    }
}
