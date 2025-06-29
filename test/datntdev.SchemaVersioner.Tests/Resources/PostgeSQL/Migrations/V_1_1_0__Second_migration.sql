-- Create simple table in postgresql
CREATE TABLE "Table2" (
	"Id" SERIAL PRIMARY KEY,
	"Name" VARCHAR(100) NOT NULL,
	"CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
-- create simple view for postgresql
CREATE VIEW "View2" AS
SELECT "Id", "Name", "CreatedAt" FROM "Table2";