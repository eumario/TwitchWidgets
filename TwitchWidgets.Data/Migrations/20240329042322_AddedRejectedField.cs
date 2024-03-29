using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchWidgets.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedRejectedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Rejected",
                table: "HeckleMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rejected",
                table: "HeckleMessages");
        }
    }
}
