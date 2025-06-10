-- create a simple view in mssql
CREATE VIEW [dbo].[View1]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table1];
GO;

-- create a simple view in mssql
CREATE VIEW [dbo].[View2]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table2];
GO;