using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELDNET.Migrations
{
    /// <inheritdoc />
    public partial class AddingBoolean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChangedByApplicant",
                table: "ReservationRooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsChangedByApplicant",
                table: "LockerRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsChangedByApplicant",
                table: "GatePasses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChangedByApplicant",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "IsChangedByApplicant",
                table: "LockerRequests");

            migrationBuilder.DropColumn(
                name: "IsChangedByApplicant",
                table: "GatePasses");
        }
    }
}
