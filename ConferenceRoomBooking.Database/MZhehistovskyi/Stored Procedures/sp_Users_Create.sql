CREATE PROCEDURE [MZhehistovskyi].[sp_Users_Create]
    @Email         NVARCHAR(256),
    @PasswordHash  NVARCHAR(512),
    @Role          NVARCHAR(32),
    @CreatedAtUtc  DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [MZhehistovskyi].[Users] ([Email], [PasswordHash], [Role], [CreatedAtUtc])
    OUTPUT INSERTED.[Id], INSERTED.[Email], INSERTED.[PasswordHash], INSERTED.[Role], INSERTED.[CreatedAtUtc]
    VALUES (@Email, @PasswordHash, @Role, @CreatedAtUtc);
END
