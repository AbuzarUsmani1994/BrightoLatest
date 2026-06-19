IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_FinancialYear')
BEGIN
    CREATE TABLE dbo.Tbl_FinancialYear
    (
        ID         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Year]     NVARCHAR(50) NOT NULL,
        CreatedOn  DATETIME NOT NULL CONSTRAINT DF_Tbl_FinancialYear_CreatedOn DEFAULT (GETDATE()),
        IsActive   BIT NOT NULL CONSTRAINT DF_Tbl_FinancialYear_IsActive DEFAULT (1)
    );
END
GO
