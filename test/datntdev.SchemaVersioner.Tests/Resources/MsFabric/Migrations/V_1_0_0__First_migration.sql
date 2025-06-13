-- Create simple table in mssql
CREATE TABLE [dbo].[Table1] (
	[Id] INT NOT NULL,
	[Name] VARCHAR(100) NOT NULL,
	[CreatedAt] DATETIME2(0) NOT NULL
);
GO
-- create a simple view in mssql
CREATE OR ALTER VIEW [dbo].[View1]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table1];