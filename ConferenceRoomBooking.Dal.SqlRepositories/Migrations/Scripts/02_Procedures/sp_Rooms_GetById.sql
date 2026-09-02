CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Rooms_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.[Id], r.[Name], r.[Capacity], r.[BaseHourRate], r.[CreatedAtUtc], r.[IsDeleted],
           so.[Id]    AS ServiceOptionId,
           so.[Name]  AS ServiceOptionName,
           so.[Price] AS ServiceOptionPrice
    FROM [MZhehistovskyi].[Rooms] r
    LEFT JOIN [MZhehistovskyi].[RoomServiceOptions] rso ON rso.[RoomId] = r.[Id]
    LEFT JOIN [MZhehistovskyi].[ServiceOptions] so ON so.[Id] = rso.[ServiceOptionId]
    WHERE r.[Id] = @Id AND r.[IsDeleted] = 0;
END
