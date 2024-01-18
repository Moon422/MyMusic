using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class changedapprovedtostatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "ArtistUpgradeRequests");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ArtistUpgradeRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ArtistUpgradeRequests");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "ArtistUpgradeRequests",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
