-- Add Setup -> Area Coverage page
IF NOT EXISTS (SELECT 1 FROM dbo.Pages WHERE PageName = 'AreaCoverage' AND ParentPageID = 3)
BEGIN
    INSERT INTO dbo.Pages (PageName, DisplayName, ParentPageID, [Type], MenuInitials, Controller, [Action], RoleType, Icon)
    VALUES ('AreaCoverage', 'Area Coverage', 3, 'Razor', '/Setup', 'Setup', 'AreaCoverage', 2, NULL);
END
GO

-- Add Setup -> Area Coverage Report page
IF NOT EXISTS (SELECT 1 FROM dbo.Pages WHERE PageName = 'AreaCoverageReport' AND ParentPageID = 3)
BEGIN
    INSERT INTO dbo.Pages (PageName, DisplayName, ParentPageID, [Type], MenuInitials, Controller, [Action], RoleType, Icon)
    VALUES ('AreaCoverageReport', 'Area Coverage Report', 3, 'Razor', '/Setup', 'Setup', 'AreaCoverageReport', 2, NULL);
END
GO
