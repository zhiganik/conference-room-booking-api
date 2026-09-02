CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Analytics_GetRoomPerformance]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [RoomId], [RoomName], [TotalBookings], [TotalRevenue], [AvgBookingDurationMinutes], [RevenueRank]
    FROM [MZhehistovskyi].[RoomPerformanceView]
    ORDER BY [RevenueRank];
END
