-- create simple table for postgresql
CREATE TABLE "Table1" (
	"Id" SERIAL PRIMARY KEY,
	"Name" VARCHAR(100) NOT NULL,
	"CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);