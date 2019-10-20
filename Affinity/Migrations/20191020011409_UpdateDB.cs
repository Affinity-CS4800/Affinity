using Microsoft.EntityFrameworkCore.Migrations;

namespace Affinity.Migrations
{
    public partial class UpdateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edges_Vertices_VertexDBID",
                table: "Edges");

            migrationBuilder.DropIndex(
                name: "IX_Edges_VertexDBID",
                table: "Edges");

            migrationBuilder.DropColumn(
                name: "VertexDBID",
                table: "Edges");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VertexDBID",
                table: "Edges",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Edges_VertexDBID",
                table: "Edges",
                column: "VertexDBID");

            migrationBuilder.AddForeignKey(
                name: "FK_Edges_Vertices_VertexDBID",
                table: "Edges",
                column: "VertexDBID",
                principalTable: "Vertices",
                principalColumn: "DBID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
