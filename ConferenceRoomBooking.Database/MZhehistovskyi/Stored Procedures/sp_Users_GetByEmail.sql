CREATE PROCEDURE [MZhehistovskyi].[sp_Users_GetByEmail]
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Email], [PasswordHash], [Role], [CreatedAtUtc]
    FROM [MZhehistovskyi].[Users]
    WHERE [Email] = @Email;
END
