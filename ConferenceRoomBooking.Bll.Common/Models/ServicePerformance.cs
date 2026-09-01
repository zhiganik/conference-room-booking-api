namespace ConferenceRoomBooking.Bll.Common.Models;

public record ServicePerformance(int Id, string Name, int TimesSelected, int DistinctRoomsUsedIn,
    decimal TotalRevenue, int RevenueRank);
