-- Create simple table in mssql
CREATE TABLE [dbo].[Table2] (
	[Id] INT NOT NULL,
	[Name] VARCHAR(100) NOT NULL,
	[CreatedAt] DATETIME2(0) NOT NULL
)
GO
-- create a simple view in mssql
CREATE VIEW [dbo].[View2]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table2]
GO