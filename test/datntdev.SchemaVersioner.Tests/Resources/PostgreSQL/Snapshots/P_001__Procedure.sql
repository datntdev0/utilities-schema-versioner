-- create a simple procedure for postgresql
CREATE OR REPLACE PROCEDURE "Procedure1"()
LANGUAGE plpgsql
AS $$
BEGIN
    -- Insert a record into Table2
    INSERT INTO "Table2" ("Name", "CreatedAt") VALUES ('New Record', CURRENT_TIMESTAMP);
END;
$$;