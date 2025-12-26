using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELDNET.Migrations
{
    /// <inheritdoc />
    public partial class Initialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacultyAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Course = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GatePasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacultyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Maker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudyLoadPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsChangedByApplicant = table.Column<bool>(type: "bit", nullable: false),
                    Approver1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver1Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver2Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver3Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver3Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatePasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LockerRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacultyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LockerNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudyLoadPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptTerms = table.Column<bool>(type: "bit", nullable: false),
                    IsChangedByApplicant = table.Column<bool>(type: "bit", nullable: false),
                    Approver1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver1Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockerRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservationRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacultyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Speaker = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Venue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurposeObjective = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateNeeded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeFrom = table.Column<TimeSpan>(type: "time", nullable: false),
                    TimeTo = table.Column<TimeSpan>(type: "time", nullable: false),
                    Participants = table.Column<int>(type: "int", nullable: false),
                    SourceOfFunds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentFacilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NatureOfActivity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReservedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver1Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver2Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver3Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver3Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approver4Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approver4Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approver5Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approver5Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Course = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAccounts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "FacultyAccounts");

            migrationBuilder.DropTable(
                name: "GatePasses");

            migrationBuilder.DropTable(
                name: "LockerRequests");

            migrationBuilder.DropTable(
                name: "ReservationRooms");

            migrationBuilder.DropTable(
                name: "StudentAccounts");
        }
    }
}
