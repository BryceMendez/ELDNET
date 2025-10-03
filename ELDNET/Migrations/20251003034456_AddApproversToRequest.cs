using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELDNET.Migrations
{
    /// <inheritdoc />
    public partial class AddApproversToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Approver1Name",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver1Status",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver2Name",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver2Status",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver3Name",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver3Status",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FinalStatus",
                table: "ReservationRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver1Name",
                table: "LockerRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver1Status",
                table: "LockerRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<string>(
                name: "FinalStatus",
                table: "LockerRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver1Name",
                table: "GatePasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver1Status",
                table: "GatePasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver2Name",
                table: "GatePasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver2Status",
                table: "GatePasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver3Name",
                table: "GatePasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Approver3Status",
                table: "GatePasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FinalStatus",
                table: "GatePasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver1Name",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver1Status",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver2Name",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver2Status",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver3Name",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver3Status",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "FinalStatus",
                table: "ReservationRooms");

            migrationBuilder.DropColumn(
                name: "Approver1Name",
                table: "LockerRequests");

            migrationBuilder.DropColumn(
                name: "Approver1Status",
                table: "LockerRequests");

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

            migrationBuilder.DropColumn(
                name: "FinalStatus",
                table: "LockerRequests");

            migrationBuilder.DropColumn(
                name: "Approver1Name",
                table: "GatePasses");

            migrationBuilder.DropColumn(
                name: "Approver1Status",
                table: "GatePasses");

            migrationBuilder.DropColumn(
                name: "Approver2Name",
                table: "GatePasses");

            migrationBuilder.DropColumn(
                name: "Approver2Status",
                table: "GatePasses");

            migrationBuilder.DropColumn(
                name: "Approver3Name",
                table: "GatePasses");

            migrationBuilder.DropColumn(
                name: "Approver3Status",
                table: "GatePasses");

            migrationBuilder.DropColumn(
                name: "FinalStatus",
                table: "GatePasses");
        }
    }
}
