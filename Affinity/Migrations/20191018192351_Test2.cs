using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Affinity.Migrations
{
    public partial class Test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VertexEdges");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Vertices",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Edges",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<int>(
                name: "VertexID",
                table: "Edges",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Edges_VertexID",
                table: "Edges",
                column: "VertexID");

            migrationBuilder.AddForeignKey(
                name: "FK_Edges_Vertices_VertexID",
                table: "Edges",
                column: "VertexID",
                principalTable: "Vertices",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edges_Vertices_VertexID",
                table: "Edges");

            migrationBuilder.DropIndex(
                name: "IX_Edges_VertexID",
                table: "Edges");

            migrationBuilder.DropColumn(
                name: "VertexID",
                table: "Edges");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Vertices",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Edges",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.CreateTable(
                name: "VertexEdges",
                columns: table => new
                {
                    VertexID = table.Column<int>(nullable: false),
                    EdgeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VertexEdges", x => new { x.VertexID, x.EdgeID });
                    table.ForeignKey(
                        name: "FK_VertexEdges_Edges_EdgeID",
                        column: x => x.EdgeID,
                        principalTable: "Edges",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VertexEdges_Vertices_VertexID",
                        column: x => x.VertexID,
                        principalTable: "Vertices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VertexEdges_EdgeID",
                table: "VertexEdges",
                column: "EdgeID");
        }
    }
}
