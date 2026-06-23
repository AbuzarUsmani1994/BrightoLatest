-- Seed SODesignations table with standard designations.
-- Idempotent: only inserts rows that don't already exist by Name.

DECLARE @desig TABLE (Name NVARCHAR(200));
INSERT INTO @desig VALUES
('Sales Manager'),
('Senior Executive Sales Admin'),
('Zonal Sales Manager'),
('Senior Manager Sales'),
('Regional Sales Manager'),
('Senior Area Sales Manager'),
('Area Sales Manager'),
('Senior Territory Manager'),
('Territory Manager'),
('Senior Sales Officer'),
('Executive'),
('Business Development Executive'),
('Senior Officer'),
('Sales Coordinator'),
('Asst. Executive ACTD'),
('Sales Officer'),
('Technical Officer'),
('Business Development Manager'),
('Sales Promotion Officer'),
('Corporate Sales Senior Territory Manager'),
('Assistant Officer'),
('Deputy Executive'),
('Corporate Sales Manager'),
('Brand Associate'),
('Junior Officer'),
('Colour Matcher'),
('Paint Applicator'),
('Officer'),
('Internee');

INSERT INTO dbo.SODesignations (Name, IsActive)
SELECT d.Name, 1
FROM @desig d
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.SODesignations s WHERE s.Name = d.Name
);
GO
