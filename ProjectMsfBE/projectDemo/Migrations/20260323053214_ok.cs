using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class ok : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Catetory_CatetoryID",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Catetory",
                table: "Catetory");

            migrationBuilder.RenameTable(
                name: "Catetory",
                newName: "Catetorys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Catetorys",
                table: "Catetorys",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 5, 32, 14, 22, DateTimeKind.Utc).AddTicks(2730));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 5, 32, 14, 22, DateTimeKind.Utc).AddTicks(2737));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 5, 32, 14, 22, DateTimeKind.Utc).AddTicks(2739));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$8pDKbHkCz3XY0gEsfeKxweL3iwFc1I2j6IGayvbfelPMN31vLFCkK");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Catetorys_CatetoryID",
                table: "Event",
                column: "CatetoryID",
                principalTable: "Catetorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Catetorys_CatetoryID",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Catetorys",
                table: "Catetorys");

            migrationBuilder.RenameTable(
                name: "Catetorys",
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
                value: new DateTime(2026, 3, 23, 3, 35, 42, 526, DateTimeKind.Utc).AddTicks(1642));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 3, 35, 42, 526, DateTimeKind.Utc).AddTicks(1664));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 23, 3, 35, 42, 526, DateTimeKind.Utc).AddTicks(1666));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$/2AA1WWI0i7KtFhnDsYL1uW2OgIWJIomLMdrzofbU5g30K1UU6KRW");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Catetory_CatetoryID",
                table: "Event",
                column: "CatetoryID",
                principalTable: "Catetory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
