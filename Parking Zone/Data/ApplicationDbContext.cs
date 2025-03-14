using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkingZone.Entities;

namespace ParkingZone.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Entities.ParkingZone> ParkingZones { get; set; } = default!;
        public DbSet<Slot> Slots { get; set; } = default!;
        public DbSet<Reservation> Reservations { get; set; }
    }
}
