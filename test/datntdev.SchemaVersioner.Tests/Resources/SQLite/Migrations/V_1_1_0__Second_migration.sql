-- Create simple table in sqlite
CREATE TABLE IF NOT EXISTS [Table2] (
	"Id" INTEGER PRIMARY KEY AUTOINCREMENT,
	"Name" TEXT NOT NULL,
	"CreatedAt" DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- create simple view for sqlite
CREATE VIEW IF NOT EXISTS [View2] AS
SELECT "Id", "Name" [Table2]