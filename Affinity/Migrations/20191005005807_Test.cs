using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Affinity.Migrations
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Edges",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    First = table.Column<int>(nullable: false),
                    Second = table.Column<int>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    Color = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edges", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Vertices",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    XPos = table.Column<int>(nullable: false),
                    YPos = table.Column<int>(nullable: false),
                    Color = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vertices", x => x.ID);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VertexEdges");

            migrationBuilder.DropTable(
                name: "Edges");

            migrationBuilder.DropTable(
                name: "Vertices");
        }
    }
}
