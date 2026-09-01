namespace ConferenceRoomBooking.Web.Dtos.Analytics;

public record ServicePerformanceResponse(int Id, string Name, int TimesSelected, int DistinctRoomsUsedIn, 
    decimal TotalRevenue, int RevenueRank);