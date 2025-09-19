-- create simple function count table record for postgresql
CREATE OR REPLACE FUNCTION "CountTableRecords"()
RETURNS INTEGER 
LANGUAGE plpgsql
AS $$
BEGIN
	RETURN (SELECT COUNT(*) FROM "Table1");
END;
$$;