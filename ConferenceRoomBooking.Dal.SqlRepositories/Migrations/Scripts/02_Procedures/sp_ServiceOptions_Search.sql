CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_Search]
    @Name NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Name], [Price]
    FROM [MZhehistovskyi].[ServiceOptions]
    WHERE @Name IS NULL OR [Name] LIKE '%' + @Name + '%'
    ORDER BY [Name];
END
