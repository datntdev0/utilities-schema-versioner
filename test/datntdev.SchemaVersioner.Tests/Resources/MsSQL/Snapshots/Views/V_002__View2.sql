-- create simple view for mssql
CREATE VIEW [dbo].[View2]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table2]

GO

-- create a simple view in mssql
CREATE VIEW [dbo].[View2_1]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table2];
