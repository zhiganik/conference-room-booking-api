-- The NZTelegramNotifier schema was never granted EXECUTE rights for the app's identity,
-- so calls to its stored procedures failed at runtime. Consolidating AlertSubscribers
-- into the default MZhehistovskyi schema, which already has the required grants.

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'AlertSubscribers' AND schema_id = SCHEMA_ID(N'MZhehistovskyi'))
BEGIN
    CREATE TABLE [MZhehistovskyi].[AlertSubscribers]
    (
        [ChatId]       BIGINT       NOT NULL,
        [CreatedAtUtc] DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT [PK_AlertSubscribers] PRIMARY KEY CLUSTERED ([ChatId])
    );
END
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = N'sp_AlertSubscribers_Add' AND schema_id = SCHEMA_ID(N'NZTelegramNotifier'))
    DROP PROCEDURE [NZTelegramNotifier].[sp_AlertSubscribers_Add];
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = N'sp_AlertSubscribers_GetAll' AND schema_id = SCHEMA_ID(N'NZTelegramNotifier'))
    DROP PROCEDURE [NZTelegramNotifier].[sp_AlertSubscribers_GetAll];
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = N'sp_AlertSubscribers_Remove' AND schema_id = SCHEMA_ID(N'NZTelegramNotifier'))
    DROP PROCEDURE [NZTelegramNotifier].[sp_AlertSubscribers_Remove];
GO

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = N'AlertSubscribers' AND schema_id = SCHEMA_ID(N'NZTelegramNotifier'))
    DROP TABLE [NZTelegramNotifier].[AlertSubscribers];
GO

IF EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'NZTelegramNotifier')
    DROP SCHEMA [NZTelegramNotifier];
GO
