-- Creates the BrightoManagers login (UserID 1123).
-- Adjust columns to match the actual Users table if needed.

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE ID = 1123)
BEGIN
    SET IDENTITY_INSERT dbo.Users ON;

    INSERT INTO dbo.Users (ID, UserName, [Password], UserType, IsActive)
    VALUES (1123, 'BrightoManagers', 'BrightoManagers@123', 'BrightoManagers', 1);

    SET IDENTITY_INSERT dbo.Users OFF;
END
ELSE
BEGIN
    UPDATE dbo.Users
       SET UserType = 'BrightoManagers',
           IsActive = 1
     WHERE ID = 1123;
END
GO
