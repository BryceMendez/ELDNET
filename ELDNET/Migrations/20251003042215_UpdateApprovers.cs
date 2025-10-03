using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELDNET.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApprovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver2Name",
                table: "LockerRequests");

            migrationBuilder.DropColumn(
                name: "Approver2Status",
                table: "LockerRequests");

            migrationBuilder.DropColumn(
                name: "Approver3Name",
                table: "LockerRequests");

            migrationBuilder.DropColumn(
                name: "Approver3Status",
                table: "LockerRequests");

            migrationBuilder.AddColumn<string>(
                name: "Approver4Name",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver4Status",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver5Name",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver5Status",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver4Name",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver4Status",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver5Name",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver5Status",
                table: "ReservationRooms");

            migrationBuilder.AddColumn<string>(
                name: "Approver2Name",
                table: "LockerRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver2Status",
                table: "LockerRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver3Name",
                table: "LockerRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver3Status",
                table: "LockerRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
