namespace ConferenceRoomBooking.Application.Dtos.Analytics;

public record RoomPerformanceResponse(int RoomId, string RoomName, int TotalBookings, decimal TotalRevenue, 
    decimal AvgBookingDurationMinutes, int RevenueRank);