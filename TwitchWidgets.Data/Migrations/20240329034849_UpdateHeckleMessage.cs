using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchWidgets.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHeckleMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "HeckleMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "HeckleMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SuggestedAt",
                table: "HeckleMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "HeckleMessages");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "HeckleMessages");

            migrationBuilder.DropColumn(
                name: "SuggestedAt",
                table: "HeckleMessages");
        }
    }
}
