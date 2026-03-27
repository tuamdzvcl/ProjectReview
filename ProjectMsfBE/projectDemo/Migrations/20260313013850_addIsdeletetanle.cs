using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    /// <inheritdoc />
    public partial class addIsdeletetanle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RolePermissions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RolePermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RolePermissions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "RolePermissions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "RolePermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Role",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Role",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Role",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Role",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Role",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Permissions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Permissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Permissions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Permissions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Permissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "CreatedBy", "CreatedDate", "IsDeleted", "UpdatedBy", "UpdatedDate" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$e0hkFSeLfG0vaqDOeybMROqR4KAc1LufJubyP9mZrqcIsyWFvkEd2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Permissions");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$eNPBeXq7g9Q2Y/G2YyylJOaa1vlNYA7GpR.uVa6iTHOOIXuPubtuq");
        }
    }
}
