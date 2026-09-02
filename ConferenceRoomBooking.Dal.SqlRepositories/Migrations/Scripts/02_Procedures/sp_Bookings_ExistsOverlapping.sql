CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Bookings_ExistsOverlapping]
    @RoomId    INT,
    @StartTime DATETIME2(3),
    @EndTime   DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM [MZhehistovskyi].[Bookings]
        WHERE [RoomId] = @RoomId AND [StartTime] < @EndTime AND [EndTime] > @StartTime
    ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [Exists];
END
