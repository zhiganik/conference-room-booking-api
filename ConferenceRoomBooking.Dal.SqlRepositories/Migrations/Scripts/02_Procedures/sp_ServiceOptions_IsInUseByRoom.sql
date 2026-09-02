CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_IsInUseByRoom]
    @ServiceOptionId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- [RoomServiceOptions] is created by the Rooms migration (next domain implemented after this
    -- one). Deferred name resolution lets this procedure be created before that table exists; it
    -- only executes for real once the Rooms feature ships the table.
    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM [MZhehistovskyi].[RoomServiceOptions] WHERE [ServiceOptionId] = @ServiceOptionId
    ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [IsInUse];
END
