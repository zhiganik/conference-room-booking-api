CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Bookings_HasActiveForRoom]
    @RoomId INT,
    @NowUtc DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM [MZhehistovskyi].[Bookings]
        WHERE [RoomId] = @RoomId AND [EndTime] > @NowUtc
    ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [Exists];
END
