namespace ConferenceRoomBooking.Bll.Common.Analytics.Models;

public record ServicePerformance(Guid Id, string Name, int TimesSelected, int DistinctRoomsUsedIn,
    decimal TotalRevenue, int RevenueRank);
