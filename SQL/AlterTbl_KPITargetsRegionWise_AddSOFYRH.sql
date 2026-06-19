IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'FinancialYearID' AND Object_ID = Object_ID('dbo.Tbl_KPITargetsRegionWise'))
BEGIN
    ALTER TABLE dbo.Tbl_KPITargetsRegionWise ADD FinancialYearID INT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'SOID' AND Object_ID = Object_ID('dbo.Tbl_KPITargetsRegionWise'))
BEGIN
    ALTER TABLE dbo.Tbl_KPITargetsRegionWise ADD SOID INT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'RegionalHeadID' AND Object_ID = Object_ID('dbo.Tbl_KPITargetsRegionWise'))
BEGIN
    ALTER TABLE dbo.Tbl_KPITargetsRegionWise ADD RegionalHeadID INT NULL;
END
GO
