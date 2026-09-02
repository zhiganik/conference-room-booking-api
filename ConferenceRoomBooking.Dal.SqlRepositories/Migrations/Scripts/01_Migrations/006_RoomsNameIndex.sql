-- Supports sp_Rooms_GetByName's exact-match lookup (WHERE Name = @Name AND IsDeleted = 0).
-- Filtered rather than covering the whole table since every current query against Rooms.Name
-- already excludes soft-deleted rows. Not unique — unlike Users.Email / ServiceOptions.Name,
-- Rooms.Name has no uniqueness business rule (matches the old EF app, which didn't enforce one
-- either), so this is a plain performance index only.
CREATE NONCLUSTERED INDEX [IX_Rooms_Name] ON [MZhehistovskyi].[Rooms] ([Name])
    WHERE [IsDeleted] = 0;
