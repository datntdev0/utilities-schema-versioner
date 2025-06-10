-- Create simple table in mssql
CREATE TABLE [dbo].[Table2] (
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Name] NVARCHAR(100) NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO;

-- create a simple view in mssql
CREATE VIEW [dbo].[View2]
AS
SELECT [Id], [Name], [CreatedAt]
FROM [dbo].[Table2];
GO;