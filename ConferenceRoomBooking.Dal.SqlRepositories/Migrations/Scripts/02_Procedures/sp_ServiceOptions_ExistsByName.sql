CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_ExistsByName]
    @Name        NVARCHAR(100),
    @ExcludingId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM [MZhehistovskyi].[ServiceOptions]
        WHERE [Name] = @Name AND (@ExcludingId IS NULL OR [Id] <> @ExcludingId)
    ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [Exists];
END
