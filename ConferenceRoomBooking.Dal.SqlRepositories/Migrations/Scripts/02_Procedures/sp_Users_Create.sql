CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Users_Create]
    @Email         NVARCHAR(256),
    @PasswordHash  NVARCHAR(512),
    @Role          NVARCHAR(32),
    @CreatedAtUtc  DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    -- Id is DB-generated (NEWSEQUENTIALID default) — handed back via OUTPUT in the same round trip.
    INSERT INTO [MZhehistovskyi].[Users] ([Email], [PasswordHash], [Role], [CreatedAtUtc])
    OUTPUT INSERTED.[Id], INSERTED.[Email], INSERTED.[PasswordHash], INSERTED.[Role], INSERTED.[CreatedAtUtc]
    VALUES (@Email, @PasswordHash, @Role, @CreatedAtUtc);
END
