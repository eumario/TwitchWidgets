using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchWidgets.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Secrets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    ClientSecret = table.Column<string>(type: "TEXT", nullable: false),
                    StreamerAuthToken = table.Column<string>(type: "TEXT", nullable: true),
                    StreamerRefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    StreamerUserId = table.Column<string>(type: "TEXT", nullable: true),
                    StreamerUserName = table.Column<string>(type: "TEXT", nullable: true),
                    StreamerDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    StreamerProfilePic = table.Column<string>(type: "TEXT", nullable: true),
                    BotAuthToken = table.Column<string>(type: "TEXT", nullable: true),
                    BotRefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    BotUserId = table.Column<string>(type: "TEXT", nullable: true),
                    BotUserName = table.Column<string>(type: "TEXT", nullable: true),
                    BotDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    BotProfilePic = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secrets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    SValue = table.Column<string>(type: "TEXT", nullable: true),
                    BValue = table.Column<bool>(type: "INTEGER", nullable: true),
                    IValue = table.Column<int>(type: "INTEGER", nullable: true),
                    FValue = table.Column<float>(type: "REAL", nullable: true),
                    DValue = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Secrets");

            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
