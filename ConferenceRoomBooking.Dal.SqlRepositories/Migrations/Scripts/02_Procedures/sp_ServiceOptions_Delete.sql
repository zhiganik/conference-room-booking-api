CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_Delete]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [MZhehistovskyi].[ServiceOptions]
    WHERE [Id] = @Id;
END
