using Microsoft.EntityFrameworkCore;
using Archi_applicatives_MSAFE.msafe.com.models;

namespace Archi_applicatives_MSAFE.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; } // Property can be plural
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Chambre> Chambres { get; set; }
    public DbSet<ChambreReservation> ChambreReservations { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChambreReservation>()
            .HasKey(cr => new { cr.ChambreId, cr.ReservationId });

        modelBuilder.Entity<ChambreReservation>()
            .HasOne(cr => cr.Chambre)
            .WithMany(c => c.ChambreReservations)
            .HasForeignKey(cr => cr.ChambreId);

        modelBuilder.Entity<ChambreReservation>()
            .HasOne(cr => cr.Reservation)
            .WithMany(r => r.ChambreReservations)
            .HasForeignKey(cr => cr.ReservationId);
    }
}