using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiorello1.Migrations
{
    public partial class HomeIntroSliderAndPhotoAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeIntroSliders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddPhotoName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeIntroSliders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeIntroSliderPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    HomeIntroSliderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeIntroSliderPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeIntroSliderPhotos_HomeIntroSliders_HomeIntroSliderId",
                        column: x => x.HomeIntroSliderId,
                        principalTable: "HomeIntroSliders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeIntroSliderPhotos_HomeIntroSliderId",
                table: "HomeIntroSliderPhotos",
                column: "HomeIntroSliderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeIntroSliderPhotos");

            migrationBuilder.DropTable(
                name: "HomeIntroSliders");
        }
    }
}
