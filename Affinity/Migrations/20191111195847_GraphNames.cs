using Microsoft.EntityFrameworkCore.Migrations;

namespace Affinity.Migrations
{
    public partial class GraphNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Direction",
                table: "Edges",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "Direction",
                table: "Edges",
                nullable: false,
                oldClrType: typeof(bool));
        }
    }
}
