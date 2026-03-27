using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class addCatetoryModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_catetories_CatetoryID",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_catetories",
                table: "catetories");

            migrationBuilder.RenameTable(
                name: "catetories",
                newName: "Catetory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Catetory",
                table: "Catetory",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 22, 23, 46, 23, 760, DateTimeKind.Utc).AddTicks(3401));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 22, 23, 46, 23, 760, DateTimeKind.Utc).AddTicks(3409));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 22, 23, 46, 23, 760, DateTimeKind.Utc).AddTicks(3412));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$1snuBfrRk6s.F5KvBWHg.uNqSLrNuPMK9FAqowmlSw11bbA3iuLGu");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Catetory_CatetoryID",
                table: "Event",
                column: "CatetoryID",
                principalTable: "Catetory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Catetory_CatetoryID",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Catetory",
                table: "Catetory");

            migrationBuilder.RenameTable(
                name: "Catetory",
                newName: "catetories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_catetories",
                table: "catetories",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Event_catetories_CatetoryID",
                table: "Event",
                column: "CatetoryID",
                principalTable: "catetories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
