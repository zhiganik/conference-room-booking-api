CREATE OR ALTER PROCEDURE [NZTelegramNotifier].[sp_AlertSubscribers_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [ChatId] FROM [NZTelegramNotifier].[AlertSubscribers];
END
