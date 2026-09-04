CREATE TABLE [MZhehistovskyi].[ServiceOptions]
(
    [Id]    INT IDENTITY(1,1) NOT NULL,
    [Name]  NVARCHAR(100)     NOT NULL,
    [Price] DECIMAL(18,2)     NOT NULL,

    CONSTRAINT [PK_ServiceOptions] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ServiceOptions_Name] ON [MZhehistovskyi].[ServiceOptions] ([Name]);
GO

CREATE TYPE [MZhehistovskyi].[IntIdList] AS TABLE
(
    [Id] INT NOT NULL PRIMARY KEY
);
