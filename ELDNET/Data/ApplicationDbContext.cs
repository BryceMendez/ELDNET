using Microsoft.EntityFrameworkCore;
using ELDNET.Models;


namespace ELDNET.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<GatePass> GatePasses { get; set; }

        public DbSet<ReservationRoom> ReservationRooms { get; set; }

        public DbSet<LockerRequest> LockerRequests { get; set; }

        public DbSet<StudentAccount> StudentAccounts { get; set; }

        public DbSet<FacultyAccount> FacultyAccounts { get; set; }

        public DbSet<Admin> Admins { get; set; }


    }
}