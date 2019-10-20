using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Affinity.Migrations
{
    public partial class InitDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UID = table.Column<string>(nullable: true),
                    GraphID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Vertices",
                columns: table => new
                {
                    DBID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GraphID = table.Column<string>(nullable: true),
                    ID = table.Column<int>(nullable: false),
                    XPos = table.Column<int>(nullable: false),
                    YPos = table.Column<int>(nullable: false),
                    Color = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vertices", x => x.DBID);
                });

            migrationBuilder.CreateTable(
                name: "Edges",
                columns: table => new
                {
                    DBID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GraphID = table.Column<string>(nullable: true),
                    ID = table.Column<int>(nullable: false),
                    First = table.Column<int>(nullable: false),
                    Second = table.Column<int>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    Color = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    VertexDBID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edges", x => x.DBID);
                    table.ForeignKey(
                        name: "FK_Edges_Vertices_VertexDBID",
                        column: x => x.VertexDBID,
                        principalTable: "Vertices",
                        principalColumn: "DBID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Edges_VertexDBID",
                table: "Edges",
                column: "VertexDBID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Edges");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vertices");
        }
    }
}
