CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Rooms_SearchAvailable]
    @Capacity  INT,
    @StartTime DATETIME2(3),
    @EndTime   DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.[Id], r.[Name], r.[Capacity], r.[BaseHourRate]
    FROM [MZhehistovskyi].[Rooms] r
    WHERE r.[IsDeleted] = 0
      AND r.[Capacity] >= @Capacity
      AND NOT EXISTS (
          SELECT 1 FROM [MZhehistovskyi].[Bookings] b
          WHERE b.[RoomId] = r.[Id]
            AND b.[StartTime] < @EndTime
            AND b.[EndTime] > @StartTime
      )
    ORDER BY r.[Name];
END
