using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELDNET.Migrations
{
    /// <inheritdoc />
    public partial class approversdenial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DenialReasonReservation",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DenialReasonLocker",
                table: "LockerRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DenialReasonReservation",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "DenialReasonLocker",
                table: "LockerRequests");
        }
    }
}
