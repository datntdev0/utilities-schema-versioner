-- create simple table for mssql
CREATE TABLE [dbo].[Table1] (
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Name] NVARCHAR(100) NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);