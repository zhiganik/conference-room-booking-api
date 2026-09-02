CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_Update]
    @Id    INT,
    @Name  NVARCHAR(100),
    @Price DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [MZhehistovskyi].[ServiceOptions]
    SET [Name] = @Name,
        [Price] = @Price
    WHERE [Id] = @Id;
END
