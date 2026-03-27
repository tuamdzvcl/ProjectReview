using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class addCatetoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CatetoryID",
                table: "Event",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "catetories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catetories", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 22, 23, 43, 5, 521, DateTimeKind.Utc).AddTicks(7028));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 22, 23, 43, 5, 521, DateTimeKind.Utc).AddTicks(7034));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 22, 23, 43, 5, 521, DateTimeKind.Utc).AddTicks(7036));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$.8ZDTBF.2F/.spwzoW5gYOOg5irkSsEeOJ1l0YIoHlMfxSk6Ju64O");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CatetoryID",
                table: "Event",
                column: "CatetoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_catetories_CatetoryID",
                table: "Event",
                column: "CatetoryID",
                principalTable: "catetories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_catetories_CatetoryID",
                table: "Event");

            migrationBuilder.DropTable(
                name: "catetories");

            migrationBuilder.DropIndex(
                name: "IX_Event_CatetoryID",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CatetoryID",
                table: "Event");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$OY5EoXBhlu1Z9.AQOxZU7OyE3eZK3DhJ7ErSAATeO3q80UdYN2UCy");
        }
    }
}
