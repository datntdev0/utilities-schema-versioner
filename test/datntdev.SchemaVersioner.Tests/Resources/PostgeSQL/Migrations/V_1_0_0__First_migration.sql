-- Create simple table in postgresql
CREATE TABLE "Table1" (
	"Id" SERIAL PRIMARY KEY,
	"Name" VARCHAR(100) NOT NULL,
	"CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
-- create a simple view in postgresql
CREATE VIEW "View1" AS
SELECT "Id", "Name", "CreatedAt" FROM "Table1";