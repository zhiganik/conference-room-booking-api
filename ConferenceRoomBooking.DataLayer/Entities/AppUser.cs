using Microsoft.AspNetCore.Identity;

namespace ConferenceRoomBooking.DataLayer.Entities;

public class AppUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}