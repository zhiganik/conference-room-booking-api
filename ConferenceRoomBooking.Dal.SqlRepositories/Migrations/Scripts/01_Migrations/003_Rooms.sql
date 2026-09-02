CREATE TABLE [MZhehistovskyi].[Rooms]
(
    [Id]           INT IDENTITY(1,1) NOT NULL,
    [Name]         NVARCHAR(200)     NOT NULL,
    [Capacity]     INT               NOT NULL,
    [BaseHourRate] DECIMAL(18,2)     NOT NULL,
    [CreatedAtUtc] DATETIME2(3)      NOT NULL DEFAULT SYSUTCDATETIME(),
    [IsDeleted]    BIT               NOT NULL DEFAULT 0,

    CONSTRAINT [PK_Rooms] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [MZhehistovskyi].[RoomServiceOptions]
(
    [RoomId]          INT NOT NULL,
    [ServiceOptionId] INT NOT NULL,

    CONSTRAINT [PK_RoomServiceOptions] PRIMARY KEY CLUSTERED ([RoomId], [ServiceOptionId]),
    CONSTRAINT [FK_RoomServiceOptions_Rooms] FOREIGN KEY ([RoomId])
        REFERENCES [MZhehistovskyi].[Rooms] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoomServiceOptions_ServiceOptions] FOREIGN KEY ([ServiceOptionId])
        REFERENCES [MZhehistovskyi].[ServiceOptions] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_RoomServiceOptions_ServiceOptionId] ON [MZhehistovskyi].[RoomServiceOptions] ([ServiceOptionId]);
