using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELDNET.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAccount",
                table: "StudentAccount");

            migrationBuilder.RenameTable(
                name: "StudentAccount",
                newName: "StudentAccounts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAccounts",
                table: "StudentAccounts",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAccounts",
                table: "StudentAccounts");

            migrationBuilder.RenameTable(
                name: "StudentAccounts",
                newName: "StudentAccount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAccount",
                table: "StudentAccount",
                column: "Id");
        }
    }
}
