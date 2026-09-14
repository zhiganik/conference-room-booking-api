CREATE OR ALTER PROCEDURE [NZTelegramNotifier].[sp_AlertSubscribers_Remove]
    @ChatId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [NZTelegramNotifier].[AlertSubscribers]
    WHERE [ChatId] = @ChatId;
END
