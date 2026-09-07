CREATE OR ALTER PROCEDURE [NZTelegramNotifier].[sp_AlertSubscribers_Add]
    @ChatId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [NZTelegramNotifier].[AlertSubscribers] WHERE [ChatId] = @ChatId)
    BEGIN
        INSERT INTO [NZTelegramNotifier].[AlertSubscribers] ([ChatId]) VALUES (@ChatId);
        SELECT CAST(1 AS BIT) AS [Inserted];
    END
    ELSE
    BEGIN
        SELECT CAST(0 AS BIT) AS [Inserted];
    END
END
