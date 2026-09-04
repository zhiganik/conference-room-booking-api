CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_GetByName]
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Name], [Price]
    FROM [MZhehistovskyi].[ServiceOptions]
    WHERE [Name] = @Name;
END
