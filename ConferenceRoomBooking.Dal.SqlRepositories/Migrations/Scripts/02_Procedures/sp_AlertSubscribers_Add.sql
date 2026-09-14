CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_AlertSubscribers_Add]
    @ChatId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [MZhehistovskyi].[AlertSubscribers] WHERE [ChatId] = @ChatId)
    BEGIN
        INSERT INTO [MZhehistovskyi].[AlertSubscribers] ([ChatId]) VALUES (@ChatId);
        SELECT CAST(1 AS BIT) AS [Inserted];
    END
    ELSE
    BEGIN
        SELECT CAST(0 AS BIT) AS [Inserted];
    END
END
