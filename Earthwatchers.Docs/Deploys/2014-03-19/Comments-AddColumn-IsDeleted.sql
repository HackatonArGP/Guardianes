Use Earthwatchers
GO

ALTER TABLE dbo.Comments ADD
	IsDeleted bit NOT NULL CONSTRAINT DF_Comments_IsDeleted DEFAULT 0
GO