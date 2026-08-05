using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkForge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkStatusAndSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ShortenedUrls",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ShortenedUrls");
        }
    }
}
