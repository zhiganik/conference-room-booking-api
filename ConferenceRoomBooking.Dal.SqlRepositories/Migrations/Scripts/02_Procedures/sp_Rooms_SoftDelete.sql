CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Rooms_SoftDelete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [MZhehistovskyi].[Rooms]
    SET [IsDeleted] = 1
    WHERE [Id] = @Id;
END
