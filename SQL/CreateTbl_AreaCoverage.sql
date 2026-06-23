IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_AreaCoverage' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_AreaCoverage
    (
        ID                INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        RegionalHeadID    INT NULL,
        SOID              INT NULL,
        RegionID          INT NULL,
        CityID            INT NULL,
        CreatedOn         DATETIME NOT NULL CONSTRAINT DF_Tbl_AreaCoverage_CreatedOn DEFAULT (GETDATE()),
        CreatedBy         INT NULL,
        IsActive          BIT NOT NULL CONSTRAINT DF_Tbl_AreaCoverage_IsActive DEFAULT (1)
    );

    CREATE INDEX IX_Tbl_AreaCoverage_SORegion ON dbo.Tbl_AreaCoverage (SOID, RegionID) WHERE IsActive = 1;
    CREATE INDEX IX_Tbl_AreaCoverage_RH ON dbo.Tbl_AreaCoverage (RegionalHeadID) WHERE IsActive = 1;
END
GO

-- If table exists from earlier version with ZoneID, rename to RegionID
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'ZoneID' AND Object_ID = Object_ID('dbo.Tbl_AreaCoverage'))
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'RegionID' AND Object_ID = Object_ID('dbo.Tbl_AreaCoverage'))
BEGIN
    EXEC sp_rename 'dbo.Tbl_AreaCoverage.ZoneID', 'RegionID', 'COLUMN';
END
GO

-- Add AreaID column (area-level coverage)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'AreaID' AND Object_ID = Object_ID('dbo.Tbl_AreaCoverage'))
BEGIN
    ALTER TABLE dbo.Tbl_AreaCoverage ADD AreaID INT NULL;
END
GO

