using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchWidgets.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImplementKnownChatter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SuggestedById",
                table: "HeckleMessages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeckleMessages_SuggestedById",
                table: "HeckleMessages",
                column: "SuggestedById");

            migrationBuilder.AddForeignKey(
                name: "FK_HeckleMessages_KnownChatters_SuggestedById",
                table: "HeckleMessages",
                column: "SuggestedById",
                principalTable: "KnownChatters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HeckleMessages_KnownChatters_SuggestedById",
                table: "HeckleMessages");

            migrationBuilder.DropIndex(
                name: "IX_HeckleMessages_SuggestedById",
                table: "HeckleMessages");

            migrationBuilder.DropColumn(
                name: "SuggestedById",
                table: "HeckleMessages");
        }
    }
}
