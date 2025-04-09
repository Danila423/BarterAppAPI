using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarterAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Listings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Listings");
        }
    }
}
