using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELDNET.Migrations
{
    /// <inheritdoc />
    public partial class ProfileEdit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "StudentAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "FacultyAccounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "StudentAccounts");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "FacultyAccounts");
        }
    }
}
