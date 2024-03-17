using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchWidgets.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHeckleMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeckleMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Heckle = table.Column<string>(type: "TEXT", nullable: false),
                    SuggestedId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeckleMessages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeckleMessages");
        }
    }
}
