CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [MZhehistovskyi].[ServiceOptions]
    WHERE [Id] = @Id;
END
