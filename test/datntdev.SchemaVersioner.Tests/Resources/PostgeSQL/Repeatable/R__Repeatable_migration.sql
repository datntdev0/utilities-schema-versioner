-- create a simple view in mssql
DROP VIEW IF EXISTS "View1_1";
CREATE VIEW "View1_1" AS
SELECT "Id", "Name", "CreatedAt" FROM "Table1";

-- create a simple view in mssql
DROP VIEW IF EXISTS "View2_1";
CREATE VIEW "View2_1" AS
SELECT "Id", "Name", "CreatedAt" FROM "Table2";
