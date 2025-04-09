using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarterAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCityColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Listings");
        }
    }
}
