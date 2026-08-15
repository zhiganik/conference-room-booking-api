using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceRoomBooking.DataLayer.Configurations;

public class RoomServiceOptionConfiguration : IEntityTypeConfiguration<RoomServiceOption>
{
    public void Configure(EntityTypeBuilder<RoomServiceOption> builder)
    {
        builder.HasKey(rs => new { rs.RoomId, rs.ServiceOptionId });
        
        builder.HasOne(rs => rs.Room)
            .WithMany(r => r.RoomServiceOptions)
            .HasForeignKey(rs => rs.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(rs => rs.ServiceOption)
            .WithMany(r => r.RoomServiceOptions)
            .HasForeignKey(rs => rs.ServiceOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}