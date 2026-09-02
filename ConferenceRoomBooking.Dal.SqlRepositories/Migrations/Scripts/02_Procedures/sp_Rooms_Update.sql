CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Rooms_Update]
    @Id               INT,
    @Name             NVARCHAR(200),
    @Capacity         INT,
    @BaseHourRate     DECIMAL(18,2),
    @ServiceOptionIds [MZhehistovskyi].[IntIdList] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    UPDATE [MZhehistovskyi].[Rooms]
    SET [Name] = @Name,
        [Capacity] = @Capacity,
        [BaseHourRate] = @BaseHourRate
    WHERE [Id] = @Id AND [IsDeleted] = 0;

    DELETE FROM [MZhehistovskyi].[RoomServiceOptions] WHERE [RoomId] = @Id;

    INSERT INTO [MZhehistovskyi].[RoomServiceOptions] ([RoomId], [ServiceOptionId])
    SELECT @Id, [Id] FROM @ServiceOptionIds;

    COMMIT TRANSACTION;
END
