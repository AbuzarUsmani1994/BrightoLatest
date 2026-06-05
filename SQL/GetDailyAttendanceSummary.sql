-- Exec GetDailyAttendanceSummary '2026-04-01','2026-05-23'

ALTER PROCEDURE GetDailyAttendanceSummary
    @DateFrom DATE,
    @DateTo   DATE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AttendanceData AS (
        SELECT
            CAST(sod.CreatedOn AS DATE)             AS [Date],
            DATENAME(WEEKDAY, sod.CreatedOn)        AS [Day],
            so.Name                                 AS [EmployeeName],
            reg.Name                                AS [Region],
            ISNULL(c.SiteName, 'No Site Selected')  AS [SiteName],
            sod.SOID,
            sod.CreatedOn                           AS [VisitTime],
            ROW_NUMBER() OVER (
                PARTITION BY CAST(sod.CreatedOn AS DATE), sod.SOID
                ORDER BY sod.CreatedOn ASC
            )                                       AS RowAsc,
            COUNT(*) OVER (
                PARTITION BY CAST(sod.CreatedOn AS DATE), sod.SOID
            )                                       AS TotalCount
        FROM Tbl_FannanSODirection sod
        INNER JOIN Saleofficers so          ON sod.SOID = so.ID
        INNER JOIN Regions reg              ON reg.ID  = so.RegionID
        INNER JOIN Tbl_FannanCustomerAssign ca
            ON sod.SOID = ca.SOID AND ca.CreatedOn <= @DateTo
        INNER JOIN Tbl_FannanCustomer c     ON ca.CustomerID = c.ID
        WHERE sod.CreatedOn BETWEEN @DateFrom AND @DateTo
          AND c.IsActive = 1
          AND ca.IsActive = 1
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY [Date], [EmployeeName], [VisitTime]) AS [Sr No],
        FORMAT([Date], 'dd-MMM-yy')   AS [Date],
        FORMAT([VisitTime], 'hh:mm')  AS [Time],
        [Day],
        [Region],
        [EmployeeName]                AS [Employee Name],
        [SiteName]                    AS [Site Name],
        CASE
            WHEN RowAsc = 1          THEN 'Day Start'
            WHEN RowAsc = TotalCount THEN 'Day End'
            ELSE 'Auto'
        END                           AS [Attendance Type]
    FROM AttendanceData
    ORDER BY [Date], [EmployeeName], [VisitTime];
END
