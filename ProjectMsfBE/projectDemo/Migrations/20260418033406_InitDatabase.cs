using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserUpgrade_UserUpgradeId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "QRCode",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserUpgradeId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserUpgrade_UserUpgradeId",
                table: "Orders",
                column: "UserUpgradeId",
                principalTable: "UserUpgrade",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserUpgrade_UserUpgradeId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "QRCode",
                table: "Ticket",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserUpgradeId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 17, 15, 48, 54, 33, DateTimeKind.Utc).AddTicks(3986));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 17, 15, 48, 54, 33, DateTimeKind.Utc).AddTicks(3997));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 17, 15, 48, 54, 33, DateTimeKind.Utc).AddTicks(4000));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$pN/qEa1hzMz0onuG7I.PIe/k/Ach8UwbnZDcAABo/4BdEOEKjKYSG");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserUpgrade_UserUpgradeId",
                table: "Orders",
                column: "UserUpgradeId",
                principalTable: "UserUpgrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
