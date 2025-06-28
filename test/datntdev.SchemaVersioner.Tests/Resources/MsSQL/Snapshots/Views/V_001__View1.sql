-- create simple view for mssql
CREATE VIEW [dbo].[View1]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table1]

GO

-- create a simple view in mssql
CREATE VIEW [dbo].[View1_1]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table1];
