-- Adds Financial Year and KPI pages under the Setup menu (ParentPageID = 3).
-- Idempotent: skips if the page already exists.

IF NOT EXISTS (SELECT 1 FROM dbo.Pages WHERE Controller = 'Setup' AND [Action] = 'FinancialYear')
BEGIN
    INSERT INTO dbo.Pages (ParentPageID, [Path], [Type], [Name], MenuInitials, Controller, [Action], ShowMenu, RoleType, Icon)
    VALUES (3, '/Setup/FinancialYear', 'Razor', 'Manage Financial Year', '/Setup', 'Setup', 'FinancialYear', 1, 2, NULL);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Pages WHERE Controller = 'Setup' AND [Action] = 'KPI')
BEGIN
    INSERT INTO dbo.Pages (ParentPageID, [Path], [Type], [Name], MenuInitials, Controller, [Action], ShowMenu, RoleType, Icon)
    VALUES (3, '/Setup/KPI', 'Razor', 'Manage KPIS', '/Setup', 'Setup', 'KPI', 1, 2, NULL);
END
GO
