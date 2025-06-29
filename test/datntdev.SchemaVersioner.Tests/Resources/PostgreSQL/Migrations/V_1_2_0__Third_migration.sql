-- create simple function count table record for postgresql
CREATE FUNCTION "CountTableRecords"()
RETURNS INTEGER 
LANGUAGE plpgsql
AS $$
BEGIN
	RETURN (SELECT COUNT(*) FROM "Table1");
END
$$;

-- create a simple procedure for postgresql
CREATE PROCEDURE "Procedure1"()
LANGUAGE plpgsql
AS $$
BEGIN
    -- Insert a record into Table2
    INSERT INTO "Table2" ("Name", "CreatedAt") VALUES ('New Record', CURRENT_TIMESTAMP);
END;
$$;