using ConferenceRoomBooking.Application.Dtos.Analytics;
using ConferenceRoomBooking.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Application.Orchestrators.Analytics;

public class AnalyticsOrchestrator(AppDbContext dbContext) : IAnalyticsOrchestrator
{
    public async Task<IReadOnlyList<RoomPerformanceResponse>> GetRoomPerformanceAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Database
            .SqlQuery<RoomPerformanceResponse>($"""
                select 
                     r.id as RoomId,
                     r.name as RoomName,
                     Count(b.id) as TotalBookings,
                     ISNULL(SUM(b.TotalPrice), 0) as TotalRevenue,
                     ISNULL(AVG(CAST(DATEDIFF(minute, b.StartTime, b.EndTime) as decimal(10,2))), 0) as AvgBookingDurationMinutes,
                     CAST(RANK() over (order by ISNULL(SUM(b.TotalPrice), 0) desc) AS INT) as RevenueRank
                from rooms as r
                left join bookings as b on r.id = b.roomId
                where r.IsDeleted = 0
                group by r.id, r.name
                order by RevenueRank;                                 
                """)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ServicePerformanceResponse>> GetServicePerformanceAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Database
            .SqlQuery<ServicePerformanceResponse>($"""
                select
                     s.id as Id,
                     s.name as Name,
                     COUNT(b.ServiceOptionId) as TimesSelected,
                     COUNT(distinct bok.RoomId) as DistinctRoomsUsedIn,
                     ISNULL(SUM(b.PriceAtBooking), 0) as TotalRevenue,
                     CAST(RANK() over (order by ISNULL(SUM(b.PriceAtBooking), 0) desc) AS INT) as RevenueRank
                from ServiceOptions s
                left join BookingServiceOptions b on s.id = b.ServiceOptionId
                left join Bookings as bok on b.BookingId = bok.Id
                group by s.id, s.name
                order by RevenueRank;                                    
                """)
            .ToListAsync(cancellationToken);
    }
}