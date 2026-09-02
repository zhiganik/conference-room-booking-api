CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_IsInUseByRoom]
    @ServiceOptionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM [MZhehistovskyi].[RoomServiceOptions] WHERE [ServiceOptionId] = @ServiceOptionId
    ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [IsInUse];
END
