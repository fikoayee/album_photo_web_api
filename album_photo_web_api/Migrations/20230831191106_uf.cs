using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace album_photo_web_api.Migrations
{
    public partial class uf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateExpired",
                table: "RefreshTokens",
                newName: "DateExpire");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateExpire",
                table: "RefreshTokens",
                newName: "DateExpired");
        }
    }
}
