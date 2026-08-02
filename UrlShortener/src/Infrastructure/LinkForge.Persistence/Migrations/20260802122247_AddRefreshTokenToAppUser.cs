using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkForge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortenedUrls_AspNetUsers_AppUserId",
                table: "ShortenedUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_AppUserId",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "ShortenedUrls");

            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UrlVisits",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Referer",
                table: "UrlVisits",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "UrlVisits",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "UrlVisits",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UrlVisits",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Referer",
                table: "UrlVisits",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "UrlVisits",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(45)",
                oldMaxLength: 45,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "UrlVisits",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppUserId",
                table: "ShortenedUrls",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_AppUserId",
                table: "ShortenedUrls",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShortenedUrls_AspNetUsers_AppUserId",
                table: "ShortenedUrls",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
