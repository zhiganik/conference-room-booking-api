CREATE OR ALTER PROCEDURE [MZhehistovskyi].[sp_Analytics_GetServicePerformance]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Name], [TimesSelected], [DistinctRoomsUsedIn], [TotalRevenue], [RevenueRank]
    FROM [MZhehistovskyi].[ServicePerformanceView]
    ORDER BY [RevenueRank];
END
