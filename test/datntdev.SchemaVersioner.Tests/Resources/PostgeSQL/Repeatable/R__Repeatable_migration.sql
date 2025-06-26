-- create a simple view in mssql
DROP VIEW IF EXISTS [dbo].[View1_1];
GO;
CREATE VIEW [dbo].[View1_1]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table1];
GO;

-- create a simple view in mssql
DROP VIEW IF EXISTS [dbo].[View2_1];
GO;
CREATE VIEW [dbo].[View2_1]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table2];
GO;