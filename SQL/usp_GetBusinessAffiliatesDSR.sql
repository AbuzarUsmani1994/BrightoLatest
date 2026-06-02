IF OBJECT_ID('dbo.usp_GetBusinessAffiliatesDSR', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetBusinessAffiliatesDSR;
GO

CREATE PROCEDURE dbo.usp_GetBusinessAffiliatesDSR
    @DateFrom DATETIME,
    @DateTo   DATETIME,
    @SOID     INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.ID                                  AS VisitID,
        v.VisitDate                           AS VisitDate,
        so.ID                                 AS SOID,
        so.Name                               AS SaleOfficerName,
        ba.ID                                 AS BusinessAffiliateID,
        ba.BusinessName                       AS BusinessName,
        ba.ContactPerson                      AS ContactPerson,
        ba.ContactNumber                      AS ContactNumber,
        ba.Address                            AS BusinessAddress,
        r.ID                                  AS RegionID,
        r.Name                                AS RegionName,
        c.ID                                  AS CityID,
        c.Name                                AS CityName,
        v.PurposeOfVisit                      AS PurposeOfVisit,
        v.TargetAgreement                     AS TargetAgreement,
        v.Remarks                             AS Remarks,
        v.CreatedDate                         AS CreatedDate
    FROM Tbl_BusinessAffiliatesVisits v
    LEFT JOIN SaleOfficers          so ON so.ID = v.SOID
    LEFT JOIN Tbl_BusinessAffiliates ba ON ba.ID = v.BusinessAffiliateID
    LEFT JOIN Regions                r  ON r.ID  = v.RegionID
    LEFT JOIN Cities                 c  ON c.ID  = v.CityID
    WHERE v.VisitDate >= @DateFrom
      AND v.VisitDate <  @DateTo
      AND (@SOID = 0 OR v.SOID = @SOID)
      AND ISNULL(v.IsActive, 1) = 1
    ORDER BY v.VisitDate, so.Name, ba.BusinessName;
END
GO
