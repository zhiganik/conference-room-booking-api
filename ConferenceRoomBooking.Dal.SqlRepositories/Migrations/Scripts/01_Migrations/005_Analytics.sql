CREATE VIEW [MZhehistovskyi].[RoomPerformanceView] AS
SELECT
    r.[Id]                                                                              AS RoomId,
    r.[Name]                                                                            AS RoomName,
    COUNT(b.[Id])                                                                       AS TotalBookings,
    ISNULL(SUM(b.[TotalPrice]), 0)                                                      AS TotalRevenue,
    ISNULL(AVG(CAST(DATEDIFF(MINUTE, b.[StartTime], b.[EndTime]) AS DECIMAL(10, 2))), 0) AS AvgBookingDurationMinutes,
    CAST(RANK() OVER (ORDER BY ISNULL(SUM(b.[TotalPrice]), 0) DESC) AS INT)              AS RevenueRank
FROM [MZhehistovskyi].[Rooms] r
LEFT JOIN [MZhehistovskyi].[Bookings] b ON r.[Id] = b.[RoomId]
WHERE r.[IsDeleted] = 0
GROUP BY r.[Id], r.[Name];
GO

CREATE VIEW [MZhehistovskyi].[ServicePerformanceView] AS
SELECT
    s.[Id]                                                                     AS Id,
    s.[Name]                                                                   AS Name,
    COUNT(bso.[ServiceOptionId])                                               AS TimesSelected,
    COUNT(DISTINCT bok.[RoomId])                                               AS DistinctRoomsUsedIn,
    ISNULL(SUM(bso.[PriceAtBooking]), 0)                                       AS TotalRevenue,
    CAST(RANK() OVER (ORDER BY ISNULL(SUM(bso.[PriceAtBooking]), 0) DESC) AS INT) AS RevenueRank
FROM [MZhehistovskyi].[ServiceOptions] s
LEFT JOIN [MZhehistovskyi].[BookingServiceOptions] bso ON s.[Id] = bso.[ServiceOptionId]
LEFT JOIN [MZhehistovskyi].[Bookings] bok ON bso.[BookingId] = bok.[Id]
GROUP BY s.[Id], s.[Name];
