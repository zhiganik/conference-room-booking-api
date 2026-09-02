CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Name], [Price]
    FROM [MZhehistovskyi].[ServiceOptions]
    WHERE [Id] = @Id;
END
