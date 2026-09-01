using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceRoomBooking.DataLayer.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.StartTime).IsRequired();
        builder.Property(b => b.EndTime).IsRequired();

        // Snapshot of Room.Name at booking time: keeps historical bookings readable/findable
        // even if the room is later renamed or soft-deleted (Room has a global query filter).
        builder.Property(b => b.RoomName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.BaseRoomCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(b => b.ServicesCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(b => b.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.CreatedAtUtc).IsRequired();

        builder.HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime });
    }
}