-- create a simple procedure for mssql
CREATE OR ALTER PROCEDURE [dbo].[Procedure1]
AS
BEGIN
	SELECT [Id], [Name], [CreatedAt]
	FROM [dbo].[Table1]
END