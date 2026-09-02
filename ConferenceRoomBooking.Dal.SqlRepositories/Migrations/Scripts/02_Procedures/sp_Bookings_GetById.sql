CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Bookings_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.[Id], b.[RoomId], b.[RoomName], b.[UserId], b.[StartTime], b.[EndTime],
           b.[BaseRoomCost], b.[ServicesCost], b.[TotalPrice], b.[CreatedAtUtc],
           bso.[ServiceOptionId], bso.[ServiceOptionName], bso.[PriceAtBooking]
    FROM [MZhehistovskyi].[Bookings] b
    LEFT JOIN [MZhehistovskyi].[BookingServiceOptions] bso ON bso.[BookingId] = b.[Id]
    WHERE b.[Id] = @Id;
END
