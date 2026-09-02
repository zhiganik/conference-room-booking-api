namespace ConferenceRoomBooking.Bll.Common.Analytics.Models;

public record RoomPerformance(Guid RoomId, string RoomName, int TotalBookings, decimal TotalRevenue,
    decimal AvgBookingDurationMinutes, int RevenueRank);
