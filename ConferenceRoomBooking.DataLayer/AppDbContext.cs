using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataLayer;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<ServiceOption> ServiceOptions => Set<ServiceOption>();
    public DbSet<RoomServiceOption> RoomServiceOptions => Set<RoomServiceOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}