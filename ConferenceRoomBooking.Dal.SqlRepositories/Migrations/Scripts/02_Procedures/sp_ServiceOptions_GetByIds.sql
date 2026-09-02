CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_ServiceOptions_GetByIds]
    @Ids [MZhehistovskyi].[GuidIdList] READONLY
AS
BEGIN
    SET NOCOUNT ON;

    SELECT so.[Id], so.[Name], so.[Price]
    FROM [MZhehistovskyi].[ServiceOptions] so
    INNER JOIN @Ids ids ON ids.[Id] = so.[Id];
END
