IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'MZhehistovskyi')
BEGIN
    EXEC('CREATE SCHEMA [MZhehistovskyi]');
END
GO

CREATE TABLE [MZhehistovskyi].[Users]
(
    [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Email]        NVARCHAR(256)    NOT NULL,
    [PasswordHash] NVARCHAR(512)    NOT NULL,
    [Role]         NVARCHAR(32)     NOT NULL,
    [CreatedAtUtc] DATETIME2(3)     NOT NULL,

    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [MZhehistovskyi].[Users] ([Email]);
