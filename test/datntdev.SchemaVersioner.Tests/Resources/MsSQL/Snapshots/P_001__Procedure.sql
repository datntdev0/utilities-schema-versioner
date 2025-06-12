-- create a simple procedure for mssql
CREATE PROCEDURE [dbo].[Procedure1]
AS
BEGIN
	SELECT [Id], [Name], [CreatedAt]
	FROM [dbo].[Table1]
END