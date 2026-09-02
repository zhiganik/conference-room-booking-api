CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Rooms_Create]
    @Name             NVARCHAR(200),
    @Capacity         INT,
    @BaseHourRate     DECIMAL(18,2),
    @ServiceOptionIds [MZhehistovskyi].[IntIdList] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @RoomIds TABLE ([Id] INT);

    BEGIN TRANSACTION;

    INSERT INTO [MZhehistovskyi].[Rooms] ([Name], [Capacity], [BaseHourRate])
    OUTPUT INSERTED.[Id] INTO @RoomIds
    VALUES (@Name, @Capacity, @BaseHourRate);

    INSERT INTO [MZhehistovskyi].[RoomServiceOptions] ([RoomId], [ServiceOptionId])
    SELECT r.[Id], ids.[Id]
    FROM @RoomIds r
    CROSS JOIN @ServiceOptionIds ids;

    COMMIT TRANSACTION;

    EXEC [MZhehistovskyi].[sp_Rooms_GetById] @Id = (SELECT [Id] FROM @RoomIds);
END
