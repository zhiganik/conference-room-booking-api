using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceRoomBooking.DataLayer.Configurations;

public class ServiceOptionConfiguration : IEntityTypeConfiguration<ServiceOption>
{
    public void Configure(EntityTypeBuilder<ServiceOption> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(s => s.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}