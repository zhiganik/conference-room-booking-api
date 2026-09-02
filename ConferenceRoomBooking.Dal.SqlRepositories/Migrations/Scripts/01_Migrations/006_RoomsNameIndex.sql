CREATE NONCLUSTERED INDEX [IX_Rooms_Name] ON [MZhehistovskyi].[Rooms] ([Name])
    WHERE [IsDeleted] = 0;
