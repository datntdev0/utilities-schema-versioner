-- create simple table for postgresql
CREATE TABLE "Table2" (
	"Id" SERIAL PRIMARY KEY,
	"Name" VARCHAR(100) NOT NULL,
	"CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);