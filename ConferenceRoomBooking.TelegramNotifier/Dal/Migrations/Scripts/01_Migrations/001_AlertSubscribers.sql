IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'NZTelegramNotifier')
BEGIN
    EXEC('CREATE SCHEMA [NZTelegramNotifier]');
END
GO

CREATE TABLE [NZTelegramNotifier].[AlertSubscribers]
(
    [ChatId]       BIGINT       NOT NULL,
    [CreatedAtUtc] DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_AlertSubscribers] PRIMARY KEY CLUSTERED ([ChatId])
);
