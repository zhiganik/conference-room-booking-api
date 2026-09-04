CREATE TABLE [MZhehistovskyi].[Bookings]
(
    [Id]           INT IDENTITY(1,1) NOT NULL,
    [RoomId]       INT               NOT NULL,
    [RoomName]     NVARCHAR(200)     NOT NULL,
    [UserId]       UNIQUEIDENTIFIER  NOT NULL,
    [StartTime]    DATETIME2(3)      NOT NULL,
    [EndTime]      DATETIME2(3)      NOT NULL,
    [BaseRoomCost] DECIMAL(18,2)     NOT NULL,
    [ServicesCost] DECIMAL(18,2)     NOT NULL,
    [TotalPrice]   DECIMAL(18,2)     NOT NULL,
    [CreatedAtUtc] DATETIME2(3)      NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_Bookings_Rooms] FOREIGN KEY ([RoomId])
        REFERENCES [MZhehistovskyi].[Rooms] ([Id]),
    CONSTRAINT [FK_Bookings_Users] FOREIGN KEY ([UserId])
        REFERENCES [MZhehistovskyi].[Users] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Bookings_UserId] ON [MZhehistovskyi].[Bookings] ([UserId]);
GO

CREATE NONCLUSTERED INDEX [IX_Bookings_RoomId_StartTime_EndTime] ON [MZhehistovskyi].[Bookings] ([RoomId], [StartTime], [EndTime]);
GO

CREATE TABLE [MZhehistovskyi].[BookingServiceOptions]
(
    [BookingId]         INT           NOT NULL,
    [ServiceOptionId]   INT           NOT NULL,
    [ServiceOptionName] NVARCHAR(100) NOT NULL,
    [PriceAtBooking]    DECIMAL(18,2) NOT NULL,

    CONSTRAINT [PK_BookingServiceOptions] PRIMARY KEY CLUSTERED ([BookingId], [ServiceOptionId]),
    CONSTRAINT [FK_BookingServiceOptions_Bookings] FOREIGN KEY ([BookingId])
        REFERENCES [MZhehistovskyi].[Bookings] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BookingServiceOptions_ServiceOptions] FOREIGN KEY ([ServiceOptionId])
        REFERENCES [MZhehistovskyi].[ServiceOptions] ([Id])
);
GO

CREATE TYPE [MZhehistovskyi].[BookingServiceOptionList] AS TABLE
(
    [ServiceOptionId]   INT           NOT NULL PRIMARY KEY,
    [ServiceOptionName] NVARCHAR(100) NOT NULL,
    [PriceAtBooking]    DECIMAL(18,2) NOT NULL
);
