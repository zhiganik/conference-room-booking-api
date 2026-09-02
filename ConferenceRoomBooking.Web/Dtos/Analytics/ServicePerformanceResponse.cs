namespace ConferenceRoomBooking.Web.Dtos.Analytics;

public record ServicePerformanceResponse(Guid Id, string Name, int TimesSelected, int DistinctRoomsUsedIn,
    decimal TotalRevenue, int RevenueRank);