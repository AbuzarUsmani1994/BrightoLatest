-- Tbl_SOAttendanceandPunctuality
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'FinancialYearID' AND Object_ID = Object_ID('dbo.Tbl_SOAttendanceandPunctuality'))
BEGIN
    ALTER TABLE dbo.Tbl_SOAttendanceandPunctuality ADD FinancialYearID INT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Quarter' AND Object_ID = Object_ID('dbo.Tbl_SOAttendanceandPunctuality'))
BEGIN
    ALTER TABLE dbo.Tbl_SOAttendanceandPunctuality ADD [Quarter] NVARCHAR(5) NULL;
END
GO

-- Tbl_SOTraining
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'FinancialYearID' AND Object_ID = Object_ID('dbo.Tbl_SOTraining'))
BEGIN
    ALTER TABLE dbo.Tbl_SOTraining ADD FinancialYearID INT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Quarter' AND Object_ID = Object_ID('dbo.Tbl_SOTraining'))
BEGIN
    ALTER TABLE dbo.Tbl_SOTraining ADD [Quarter] NVARCHAR(5) NULL;
END
GO
