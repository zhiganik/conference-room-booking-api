CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_AlertSubscribers_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [ChatId] FROM [MZhehistovskyi].[AlertSubscribers];
END
