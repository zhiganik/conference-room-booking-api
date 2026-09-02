CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Bookings_Create]
    @RoomId         UNIQUEIDENTIFIER,
    @RoomName       NVARCHAR(200),
    @UserId         UNIQUEIDENTIFIER,
    @StartTime      DATETIME2(3),
    @EndTime        DATETIME2(3),
    @BaseRoomCost   DECIMAL(18,2),
    @ServicesCost   DECIMAL(18,2),
    @TotalPrice     DECIMAL(18,2),
    @ServiceOptions [MZhehistovskyi].[BookingServiceOptionList] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @BookingIds TABLE ([Id] UNIQUEIDENTIFIER);

    BEGIN TRANSACTION;

    INSERT INTO [MZhehistovskyi].[Bookings]
        ([RoomId], [RoomName], [UserId], [StartTime], [EndTime], [BaseRoomCost], [ServicesCost], [TotalPrice])
    OUTPUT INSERTED.[Id] INTO @BookingIds
    VALUES (@RoomId, @RoomName, @UserId, @StartTime, @EndTime, @BaseRoomCost, @ServicesCost, @TotalPrice);

    INSERT INTO [MZhehistovskyi].[BookingServiceOptions] ([BookingId], [ServiceOptionId], [ServiceOptionName], [PriceAtBooking])
    SELECT b.[Id], so.[ServiceOptionId], so.[ServiceOptionName], so.[PriceAtBooking]
    FROM @BookingIds b
    CROSS JOIN @ServiceOptions so;

    COMMIT TRANSACTION;

    SELECT [Id] FROM @BookingIds;
END
