-- Converts every surrogate INT identity key (Rooms, ServiceOptions, Bookings, and the
-- RoomServiceOptions/BookingServiceOptions link tables) to UNIQUEIDENTIFIER, preserving existing
-- rows and their relationships. Users already uses UNIQUEIDENTIFIER and is untouched.
--
-- Strategy per table: add a new GUID column (backfilled via DEFAULT NEWSEQUENTIALID() for owning
-- tables, or via a join to the parent's new GUID for FK columns), drop the old INT column and its
-- constraints/indexes, rename the new column into place, then recreate constraints/indexes.
-- INT -> UNIQUEIDENTIFIER has no implicit or explicit conversion, so this can't be a plain
-- ALTER COLUMN; new values must be generated and propagated to dependents before the old columns
-- are dropped.

-- ===== 1. Drop foreign keys referencing the columns being replaced =====
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] DROP CONSTRAINT [FK_RoomServiceOptions_Rooms];
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] DROP CONSTRAINT [FK_RoomServiceOptions_ServiceOptions];
ALTER TABLE [MZhehistovskyi].[Bookings] DROP CONSTRAINT [FK_Bookings_Rooms];
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] DROP CONSTRAINT [FK_BookingServiceOptions_Bookings];
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] DROP CONSTRAINT [FK_BookingServiceOptions_ServiceOptions];
GO

-- ===== 2. Drop indexes that index the columns being replaced =====
DROP INDEX [IX_RoomServiceOptions_ServiceOptionId] ON [MZhehistovskyi].[RoomServiceOptions];
DROP INDEX [IX_Bookings_RoomId_StartTime_EndTime] ON [MZhehistovskyi].[Bookings];
GO

-- ===== 3. Drop primary keys on the columns being replaced =====
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] DROP CONSTRAINT [PK_RoomServiceOptions];
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] DROP CONSTRAINT [PK_BookingServiceOptions];
ALTER TABLE [MZhehistovskyi].[Rooms] DROP CONSTRAINT [PK_Rooms];
ALTER TABLE [MZhehistovskyi].[ServiceOptions] DROP CONSTRAINT [PK_ServiceOptions];
ALTER TABLE [MZhehistovskyi].[Bookings] DROP CONSTRAINT [PK_Bookings];
GO

-- ===== 4. Add new GUID columns =====
-- Owning tables: a fresh id per existing row, generated immediately by the DEFAULT.
ALTER TABLE [MZhehistovskyi].[Rooms] ADD [NewId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID();
ALTER TABLE [MZhehistovskyi].[ServiceOptions] ADD [NewId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID();
ALTER TABLE [MZhehistovskyi].[Bookings] ADD [NewId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID();

-- Foreign-key columns: nullable for now, populated from the parent's NewId below.
ALTER TABLE [MZhehistovskyi].[Bookings] ADD [NewRoomId] UNIQUEIDENTIFIER NULL;
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] ADD [NewRoomId] UNIQUEIDENTIFIER NULL;
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] ADD [NewServiceOptionId] UNIQUEIDENTIFIER NULL;
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] ADD [NewBookingId] UNIQUEIDENTIFIER NULL;
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] ADD [NewServiceOptionId] UNIQUEIDENTIFIER NULL;
GO

-- ===== 5. Propagate the new parent ids down to existing child rows via the old INT FK values =====
UPDATE b
SET b.[NewRoomId] = r.[NewId]
FROM [MZhehistovskyi].[Bookings] b
INNER JOIN [MZhehistovskyi].[Rooms] r ON b.[RoomId] = r.[Id];

UPDATE rso
SET rso.[NewRoomId] = r.[NewId]
FROM [MZhehistovskyi].[RoomServiceOptions] rso
INNER JOIN [MZhehistovskyi].[Rooms] r ON rso.[RoomId] = r.[Id];

UPDATE rso
SET rso.[NewServiceOptionId] = so.[NewId]
FROM [MZhehistovskyi].[RoomServiceOptions] rso
INNER JOIN [MZhehistovskyi].[ServiceOptions] so ON rso.[ServiceOptionId] = so.[Id];

UPDATE bso
SET bso.[NewBookingId] = bk.[NewId]
FROM [MZhehistovskyi].[BookingServiceOptions] bso
INNER JOIN [MZhehistovskyi].[Bookings] bk ON bso.[BookingId] = bk.[Id];

UPDATE bso
SET bso.[NewServiceOptionId] = so.[NewId]
FROM [MZhehistovskyi].[BookingServiceOptions] bso
INNER JOIN [MZhehistovskyi].[ServiceOptions] so ON bso.[ServiceOptionId] = so.[Id];
GO

-- ===== 6. Now that every row has its new FK value, make the columns required =====
ALTER TABLE [MZhehistovskyi].[Bookings] ALTER COLUMN [NewRoomId] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] ALTER COLUMN [NewRoomId] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] ALTER COLUMN [NewServiceOptionId] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] ALTER COLUMN [NewBookingId] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] ALTER COLUMN [NewServiceOptionId] UNIQUEIDENTIFIER NOT NULL;
GO

-- ===== 7. Drop the old INT columns =====
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] DROP COLUMN [RoomId], [ServiceOptionId];
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] DROP COLUMN [BookingId], [ServiceOptionId];
ALTER TABLE [MZhehistovskyi].[Bookings] DROP COLUMN [RoomId];
ALTER TABLE [MZhehistovskyi].[Rooms] DROP COLUMN [Id];
ALTER TABLE [MZhehistovskyi].[ServiceOptions] DROP COLUMN [Id];
ALTER TABLE [MZhehistovskyi].[Bookings] DROP COLUMN [Id];
GO

-- ===== 8. Rename the new columns into place =====
EXEC sp_rename N'MZhehistovskyi.Rooms.NewId', N'Id', 'COLUMN';
EXEC sp_rename N'MZhehistovskyi.ServiceOptions.NewId', N'Id', 'COLUMN';
EXEC sp_rename N'MZhehistovskyi.Bookings.NewId', N'Id', 'COLUMN';
EXEC sp_rename N'MZhehistovskyi.Bookings.NewRoomId', N'RoomId', 'COLUMN';
EXEC sp_rename N'MZhehistovskyi.RoomServiceOptions.NewRoomId', N'RoomId', 'COLUMN';
EXEC sp_rename N'MZhehistovskyi.RoomServiceOptions.NewServiceOptionId', N'ServiceOptionId', 'COLUMN';
EXEC sp_rename N'MZhehistovskyi.BookingServiceOptions.NewBookingId', N'BookingId', 'COLUMN';
EXEC sp_rename N'MZhehistovskyi.BookingServiceOptions.NewServiceOptionId', N'ServiceOptionId', 'COLUMN';
GO

-- ===== 9. Re-add primary keys =====
ALTER TABLE [MZhehistovskyi].[Rooms] ADD CONSTRAINT [PK_Rooms] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [MZhehistovskyi].[ServiceOptions] ADD CONSTRAINT [PK_ServiceOptions] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [MZhehistovskyi].[Bookings] ADD CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] ADD CONSTRAINT [PK_RoomServiceOptions] PRIMARY KEY CLUSTERED ([RoomId], [ServiceOptionId]);
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] ADD CONSTRAINT [PK_BookingServiceOptions] PRIMARY KEY CLUSTERED ([BookingId], [ServiceOptionId]);
GO

-- ===== 10. Re-add foreign keys =====
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] ADD CONSTRAINT [FK_RoomServiceOptions_Rooms] FOREIGN KEY ([RoomId])
    REFERENCES [MZhehistovskyi].[Rooms] ([Id]) ON DELETE CASCADE;
ALTER TABLE [MZhehistovskyi].[RoomServiceOptions] ADD CONSTRAINT [FK_RoomServiceOptions_ServiceOptions] FOREIGN KEY ([ServiceOptionId])
    REFERENCES [MZhehistovskyi].[ServiceOptions] ([Id]);
ALTER TABLE [MZhehistovskyi].[Bookings] ADD CONSTRAINT [FK_Bookings_Rooms] FOREIGN KEY ([RoomId])
    REFERENCES [MZhehistovskyi].[Rooms] ([Id]);
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] ADD CONSTRAINT [FK_BookingServiceOptions_Bookings] FOREIGN KEY ([BookingId])
    REFERENCES [MZhehistovskyi].[Bookings] ([Id]) ON DELETE CASCADE;
ALTER TABLE [MZhehistovskyi].[BookingServiceOptions] ADD CONSTRAINT [FK_BookingServiceOptions_ServiceOptions] FOREIGN KEY ([ServiceOptionId])
    REFERENCES [MZhehistovskyi].[ServiceOptions] ([Id]);
GO

-- ===== 11. Re-add indexes =====
CREATE NONCLUSTERED INDEX [IX_RoomServiceOptions_ServiceOptionId] ON [MZhehistovskyi].[RoomServiceOptions] ([ServiceOptionId]);
CREATE NONCLUSTERED INDEX [IX_Bookings_RoomId_StartTime_EndTime] ON [MZhehistovskyi].[Bookings] ([RoomId], [StartTime], [EndTime]);
GO

-- ===== 12. Table-valued parameter types: INT-keyed types can't be ALTERed, so drop and recreate.
-- SQL Server refuses to drop a type while a procedure signature still references it, so drop the
-- affected procedures first — the 02_Procedures scripts redeploy them (with GUID parameters)
-- immediately after this migration runs. =====
DROP PROCEDURE IF EXISTS [MZhehistovskyi].[sp_Rooms_Create];
DROP PROCEDURE IF EXISTS [MZhehistovskyi].[sp_Rooms_Update];
DROP PROCEDURE IF EXISTS [MZhehistovskyi].[sp_ServiceOptions_GetByIds];
DROP PROCEDURE IF EXISTS [MZhehistovskyi].[sp_Bookings_Create];
GO

DROP TYPE [MZhehistovskyi].[IntIdList];
DROP TYPE [MZhehistovskyi].[BookingServiceOptionList];
GO

CREATE TYPE [MZhehistovskyi].[GuidIdList] AS TABLE
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
);
GO

CREATE TYPE [MZhehistovskyi].[BookingServiceOptionList] AS TABLE
(
    [ServiceOptionId]   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [ServiceOptionName] NVARCHAR(100)    NOT NULL,
    [PriceAtBooking]    DECIMAL(18,2)    NOT NULL
);
GO
