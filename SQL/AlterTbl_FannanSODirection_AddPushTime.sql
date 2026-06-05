IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE Name = N'PushTime'
      AND Object_ID = Object_ID(N'dbo.Tbl_FannanSODirection')
)
BEGIN
    ALTER TABLE dbo.Tbl_FannanSODirection
    ADD PushTime DATETIME NULL;
END
GO
