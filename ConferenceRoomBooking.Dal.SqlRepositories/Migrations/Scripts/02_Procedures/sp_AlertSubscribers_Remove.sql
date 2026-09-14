CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_AlertSubscribers_Remove]
    @ChatId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [MZhehistovskyi].[AlertSubscribers]
    WHERE [ChatId] = @ChatId;
END
