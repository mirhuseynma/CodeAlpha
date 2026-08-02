using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkForge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_CustomAlias",
                table: "ShortenedUrls",
                column: "CustomAlias",
                unique: true,
                filter: "\"CustomAlias\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_CustomAlias",
                table: "ShortenedUrls");
        }
    }
}
