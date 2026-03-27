using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class addpermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissonsName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PermissonsDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 27, 9, 36, 3, 835, DateTimeKind.Utc).AddTicks(8066), new DateTime(2026, 2, 27, 9, 36, 3, 835, DateTimeKind.Utc).AddTicks(8070) });

            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 27, 9, 36, 3, 835, DateTimeKind.Utc).AddTicks(8090), "$2a$11$t4aQqv2or5idjt3ldRawW.dQUhcMVsD6hLEQDLEploYNz5OaPqTRC", new DateTime(2026, 2, 27, 9, 36, 3, 835, DateTimeKind.Utc).AddTicks(8090) });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 27, 8, 27, 32, 705, DateTimeKind.Utc).AddTicks(1432), new DateTime(2026, 2, 27, 8, 27, 32, 705, DateTimeKind.Utc).AddTicks(1437) });

            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 2, 27, 8, 27, 32, 705, DateTimeKind.Utc).AddTicks(1458), "$2a$11$Wyj.FXEpmA0G3BQZhG.AsulGe.xRPc4NLc88ucafTeoctgqwD0v5W", new DateTime(2026, 2, 27, 8, 27, 32, 705, DateTimeKind.Utc).AddTicks(1459) });
        }
    }
}
