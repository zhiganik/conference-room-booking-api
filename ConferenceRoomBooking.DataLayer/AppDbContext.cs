using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataLayer;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<ServiceOption> ServiceOptions => Set<ServiceOption>();
    public DbSet<RoomServiceOption> RoomServiceOptions => Set<RoomServiceOption>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingServiceOption> BookingServiceOptions => Set<BookingServiceOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}