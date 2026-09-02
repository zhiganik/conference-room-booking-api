namespace ConferenceRoomBooking.Web.Dtos.Analytics;

public record RoomPerformanceResponse(Guid RoomId, string RoomName, int TotalBookings, decimal TotalRevenue,
    decimal AvgBookingDurationMinutes, int RevenueRank);