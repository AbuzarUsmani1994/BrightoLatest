-- Run this script once in the database before using the Quarter Setup screen

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Tbl_Quarters') AND type = 'U')
BEGIN
    CREATE TABLE dbo.Tbl_Quarters (
        ID              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name            NVARCHAR(100)     NOT NULL,
        StartDate       DATE              NOT NULL,
        EndDate         DATE              NOT NULL,
        FinancialYearID INT               NOT NULL,
        IsActive        BIT               NOT NULL DEFAULT 1,
        IsDeleted       BIT               NOT NULL DEFAULT 0,
        CreatedOn       DATETIME          NOT NULL DEFAULT GETDATE()
    );
END
