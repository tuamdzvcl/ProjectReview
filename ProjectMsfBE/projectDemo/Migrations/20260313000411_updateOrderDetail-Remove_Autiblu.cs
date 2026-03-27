using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderDetailRemove_Autiblu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "OrderDetail");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$eNPBeXq7g9Q2Y/G2YyylJOaa1vlNYA7GpR.uVa6iTHOOIXuPubtuq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "OrderDetail",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OrderDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderDetail",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "OrderDetail",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "OrderDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$OABloXQ2wOHUby2eNqVRB.h42I6W3Zswyl0dTkG.wiUHAVc73nc3K");
        }
    }
}
