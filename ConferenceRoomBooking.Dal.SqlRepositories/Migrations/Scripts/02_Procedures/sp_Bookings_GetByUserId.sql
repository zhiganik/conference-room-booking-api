CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Bookings_GetByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.[Id], b.[RoomId], b.[RoomName], b.[UserId], b.[StartTime], b.[EndTime],
           b.[BaseRoomCost], b.[ServicesCost], b.[TotalPrice], b.[CreatedAtUtc],
           bso.[ServiceOptionId], bso.[ServiceOptionName], bso.[PriceAtBooking]
    FROM [MZhehistovskyi].[Bookings] b
    LEFT JOIN [MZhehistovskyi].[BookingServiceOptions] bso ON bso.[BookingId] = b.[Id]
    WHERE b.[UserId] = @UserId
    ORDER BY b.[StartTime] DESC, b.[Id], bso.[ServiceOptionId];
END
