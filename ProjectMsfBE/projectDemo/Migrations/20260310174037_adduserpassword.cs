using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class adduserpassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "UserLogin");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$NTnQREr9.eRm0zvgoojhc.WOQ2nBc.br6OlvXrxRUV5vD4gkuwfM.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "UserLogin",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.InsertData(
                table: "UserLogin",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsDeleted", "PasswordHash", "Provider", "ProviderUserId", "UpdatedBy", "UpdatedDate", "UserId" },
                values: new object[] { 1, null, null, false, "$2a$11$RbrANq5ASNElSPB01XpQUu6/Kz9b5KEogeOoug0oS0BSEWQGYAFWm", null, null, null, null, new Guid("11111111-1111-1111-1111-111111111111") });
        }
    }
}
