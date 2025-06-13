-- create simple function count table record for mssql
CREATE FUNCTION [dbo].[CountTableRecords] ()
RETURNS INT
AS
BEGIN
	DECLARE @Count INT;
	SELECT @Count = COUNT(*) FROM [dbo].[Table1];
	RETURN @Count;
END;