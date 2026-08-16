using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceRoomBooking.DataLayer.Configurations;

public class BookingServiceOptionConfiguration : IEntityTypeConfiguration<BookingServiceOption>
{
    public void Configure(EntityTypeBuilder<BookingServiceOption> builder)
    {
        builder.HasKey(bso => new { bso.BookingId, bso.ServiceOptionId });

        builder.Property(bso => bso.PriceAtBooking)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(bso => bso.Booking)
            .WithMany(b => b.BookingServiceOptions)
            .HasForeignKey(bso => bso.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bso => bso.ServiceOption)
            .WithMany()
            .HasForeignKey(bso => bso.ServiceOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}