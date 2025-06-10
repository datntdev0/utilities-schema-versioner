-- create simple function count table record for mssql
CREATE FUNCTION [dbo].[CountTableRecords] ()
RETURNS INT
AS
BEGIN
	DECLARE @Count INT;
	SELECT @Count = COUNT(*) FROM [dbo].[Table1];
	RETURN @Count;
END;
GO;

-- create a simple procedure for mssql
CREATE PROCEDURE [dbo].[Procedure1]
AS
BEGIN
	SELECT [Id], [Name], [CreatedAt]
	FROM [dbo].[Table1]
END
GO;