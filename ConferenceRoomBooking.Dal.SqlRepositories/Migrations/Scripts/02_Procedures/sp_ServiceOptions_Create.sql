CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_Create]
    @Name  NVARCHAR(100),
    @Price DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [MZhehistovskyi].[ServiceOptions] ([Name], [Price])
    OUTPUT INSERTED.[Id], INSERTED.[Name], INSERTED.[Price]
    VALUES (@Name, @Price);
END
