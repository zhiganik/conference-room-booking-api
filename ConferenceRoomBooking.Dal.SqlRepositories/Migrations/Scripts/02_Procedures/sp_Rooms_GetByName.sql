CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Rooms_GetByName]
    @Name NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RoomId UNIQUEIDENTIFIER = (
        SELECT TOP (1) [Id] FROM [MZhehistovskyi].[Rooms]
        WHERE [Name] = @Name AND [IsDeleted] = 0
        ORDER BY [Id]
    );

    SELECT r.[Id], r.[Name], r.[Capacity], r.[BaseHourRate], r.[CreatedAtUtc], r.[IsDeleted],
           so.[Id]    AS ServiceOptionId,
           so.[Name]  AS ServiceOptionName,
           so.[Price] AS ServiceOptionPrice
    FROM [MZhehistovskyi].[Rooms] r
    LEFT JOIN [MZhehistovskyi].[RoomServiceOptions] rso ON rso.[RoomId] = r.[Id]
    LEFT JOIN [MZhehistovskyi].[ServiceOptions] so ON so.[Id] = rso.[ServiceOptionId]
    WHERE r.[Id] = @RoomId;
END
