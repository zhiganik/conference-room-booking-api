CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Bookings_GetCreatedBetween]
    @FromUtc DATETIME2,
    @ToUtc DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.[Id], b.[RoomId], b.[RoomName], b.[UserId], b.[StartTime], b.[EndTime],
           b.[BaseRoomCost], b.[ServicesCost], b.[TotalPrice], b.[CreatedAtUtc],
           bso.[ServiceOptionId], bso.[ServiceOptionName], bso.[PriceAtBooking]
    FROM [MZhehistovskyi].[Bookings] b
    LEFT JOIN [MZhehistovskyi].[BookingServiceOptions] bso ON bso.[BookingId] = b.[Id]
    WHERE b.[CreatedAtUtc] >= @FromUtc AND b.[CreatedAtUtc] < @ToUtc
    ORDER BY b.[CreatedAtUtc], b.[Id], bso.[ServiceOptionId];
END
