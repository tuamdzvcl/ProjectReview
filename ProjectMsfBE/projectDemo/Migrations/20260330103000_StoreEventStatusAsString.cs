using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectDemo.Migrations
{
    public partial class StoreEventStatusAsString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusTemp",
                table: "Event",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DRAFT");

            migrationBuilder.Sql(@"
                UPDATE [Event]
                SET [StatusTemp] = CASE [Status]
                    WHEN 1 THEN 'DRAFT'
                    WHEN 2 THEN 'PUBLISHED'
                    WHEN 3 THEN 'CANNEL'
                    ELSE 'DRAFT'
                END
            ");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Event",
                newName: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(@"
                UPDATE [Event]
                SET [StatusTemp] = CASE [Status]
                    WHEN 'DRAFT' THEN 1
                    WHEN 'PUBLISHED' THEN 2
                    WHEN 'CANNEL' THEN 3
                    ELSE 1
                END
            ");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Event",
                newName: "Status");
        }
    }
}
