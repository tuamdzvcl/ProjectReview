using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class createDate_Updateby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { null, "$2a$11$2NrJmSC3biIEAr8blTFWOOupfNq2Yshp44t6XM0IDme01NH9nammG", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 3, 2, 8, 36, 11, 184, DateTimeKind.Utc).AddTicks(4840), new DateTime(2026, 3, 2, 8, 36, 11, 184, DateTimeKind.Utc).AddTicks(4847) });

            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 3, 2, 8, 36, 11, 184, DateTimeKind.Utc).AddTicks(4865), "$2a$11$9GrBLBFpueYoXyxaq1ZEduAhBP.bPIPqv9GHJKcJa2/pYvgApki1O", new DateTime(2026, 3, 2, 8, 36, 11, 184, DateTimeKind.Utc).AddTicks(4866) });
        }
    }
}
